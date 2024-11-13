using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Vogen;

internal class GenerateCodeForMessagePack
{
    public static void GenerateForAMarkerClass(SourceProductionContext context,
        Compilation compilation,
        MarkerClassDefinition markerClass,
        VogenKnownSymbols vogenKnownSymbols)
    {
        var markerClassSymbol = markerClass.MarkerClassSymbol;

        if (!markerClass.AttributeDefinitions.Any(m => m.Marker?.Kind is ConversionMarkerKind.MessagePack))
        {
            return;
        }

        string pns = markerClassSymbol.FullNamespace() ?? "";

        string ns = pns.Length == 0 ? "" : $"namespace {pns};";

        var isPublic = markerClassSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";


        var s = $$"""
                  {{GeneratedCodeSegments.Preamble}}

                  {{ns}}

                  {{accessor}} partial class {{markerClassSymbol.Name}}
                  {
                     {{GenerateManifest()}}
                     {{GenerateFormatters()}}
                  }

                  """;

        SourceText sourceText = Util.FormatSource(s);

        string filename = Util.GetLegalMarkerClassFilename(markerClass.MarkerClassSymbol, ConversionMarkerKind.MessagePack);

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);

        return;

        string GenerateManifest()
        {
            return
                $$"""
                  {{accessor}} static global::MessagePack.Formatters.IMessagePackFormatter[] MessagePackFormatters => new global::MessagePack.Formatters.IMessagePackFormatter[]
                  {
                      {{GenerateEach()}}
                  };
                  """;

            string GenerateEach()
            {
                string?[] names = markerClass.AttributeDefinitions.Where(
                    m => m.Marker?.Kind is ConversionMarkerKind.MessagePack).Select(
                    x =>
                    {
                        if (x is null) return null;
                        if (x.Marker is null) return null;
                        
                        string? wrapperNameShort = x.Marker.VoSymbol.Name;
                        
                        return $"new {wrapperNameShort}MessagePackFormatter()";
                    }).ToArray();

                return string.Join(", ", names);
            }
        }

        string GenerateFormatters()
        {
            StringBuilder sb = new();

            foreach (MarkerAttributeDefinition eachMarker in markerClass.AttributeDefinitions.Where(
                         m => m.Marker?.Kind is ConversionMarkerKind.MessagePack))
            {
                sb.AppendLine(
                    $$"""
                          {{GenerateSource("public", eachMarker.Marker!.VoSymbol, eachMarker.Marker.UnderlyingTypeSymbol, vogenKnownSymbols)}}
                      """);
            }

            return sb.ToString();
        }
    }

    public static void GenerateForApplicableValueObjects(SourceProductionContext context,
        Compilation compilation,
        List<VoWorkItem> valueObjects,
        VogenKnownSymbols knownSymbols)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }

        var matchingVos = valueObjects.Where(i => i.Config.Conversions.HasFlag(Conversions.MessagePack)).ToList();

        List<MessagePackStandalone> items = matchingVos.Select(MessagePackStandalone.FromWorkItem).ToList();

        List<FormatterSourceAndFilename> toWrite = items.Select(
            p => GenerateSourceAndFilename(p.WrapperAccessibility, p.WrapperType, p.ContainerNamespace, p.UnderlyingType, knownSymbols)).ToList();

        foreach (var eachToWrite in toWrite)
        {
            SourceText sourceText = Util.FormatSource(eachToWrite.SourceCode);

            Util.TryWriteUsingUniqueFilename(eachToWrite.Filename, context, sourceText);
        }
    }

    public record FormatterSourceAndFilename(string FormatterFullyQualifiedName, string Filename, string SourceCode);

    private static FormatterSourceAndFilename GenerateSourceAndFilename(string accessibility,
        INamedTypeSymbol wrapperSymbol,
        string theNamespace,
        INamedTypeSymbol underlyingSymbol,
        VogenKnownSymbols knownSymbols)
    {
        string wrapperName = Util.EscapeIfRequired(wrapperSymbol.Name);

        var ns = string.IsNullOrEmpty(theNamespace) ? string.Empty : $"namespace {theNamespace};";

        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              {{ns}}

              {{GenerateSource(accessibility, wrapperSymbol, underlyingSymbol, knownSymbols)}}          
              """;

        var fn = string.IsNullOrEmpty(theNamespace) ? "" : theNamespace + ".";
        string serializerFqn = $"{fn}{wrapperName}MessagePackFormatter";

        var unsanitized = $"{wrapperSymbol.ToDisplayString()}_messagepack.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new FormatterSourceAndFilename(serializerFqn, filename, sb);
    }


    private static string GenerateSource(string accessibility,
        INamedTypeSymbol wrapperSymbol,
        INamedTypeSymbol underlyingSymbol,
        VogenKnownSymbols vogenKnownSymbols)
    {
        var accessor = accessibility;

        string wrapperNameShort = Util.EscapeIfRequired(wrapperSymbol.Name);
        string wrapperName = Util.EscapeIfRequired(wrapperSymbol.FullName() ?? wrapperSymbol.Name);

        string underlyingTypeName = underlyingSymbol.FullName() ?? underlyingSymbol.Name;
        
        // if (readAndWriteMethods.Item1.Length == 0)
        // {
        //     return $"#error unsupported underlying type '{underlyingSymbol.FullName()}' for value object '{wrapperSymbol.Name}' - you need to turn off MessagePack support for this value object and provide your own resolver";
        // }

        string nativeReadMethod = TryGetNativeReadMethod(underlyingSymbol);

        if (!string.IsNullOrEmpty(nativeReadMethod))
        {
            return $$"""
                     {{accessor}} partial class {{wrapperNameShort}}MessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{{wrapperName}}>
                     {
                         public void Serialize(ref global::MessagePack.MessagePackWriter writer, {{wrapperName}} value, global::MessagePack.MessagePackSerializerOptions options) =>
                             writer.Write(value.Value);
                     
                         public {{wrapperName}} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options) =>
                             Deserialize(reader.{{nativeReadMethod}});
                     
                       static {{wrapperName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                       
                       [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                       static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
                     }
                     """;
        }

        return $$"""
                 {{accessor}} partial class {{wrapperNameShort}}MessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{{wrapperName}}>
                 {
                     public void Serialize(ref global::MessagePack.MessagePackWriter writer, {{wrapperName}} value, global::MessagePack.MessagePackSerializerOptions options)
                     {
                         global::MessagePack.Formatters.IMessagePackFormatter<{{underlyingTypeName}}>? r = options.Resolver.GetFormatter<{{underlyingTypeName}}>();
                         if (r is null) Throw("No formatter for underlying type of '{{underlyingTypeName}}' registered for value object '{{wrapperName}}'.");
                         r.Serialize(ref writer, value.Value, options);
                     }
                 
                     public {{wrapperName}} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
                     {
                         global::MessagePack.Formatters.IMessagePackFormatter<{{underlyingTypeName}}>? r = options.Resolver.GetFormatter<{{underlyingTypeName}}>();
                         if (r is null) Throw("No formatter for underlying type of '{{underlyingTypeName}}' registered for value object '{{wrapperName}}'.");
                         {{underlyingTypeName}} g = r.Deserialize(ref reader, options);
                         return Deserialize(g);
                     }
                     
                     private static void Throw(string message) => throw new global::MessagePack.MessagePackSerializationException(message);
                 
                    static {{wrapperName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                   
                    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                    static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
                 }
                 """;
    }

    private static string TryGetNativeReadMethod(INamedTypeSymbol primitive) =>
        primitive.SpecialType switch
        {
            SpecialType.System_Boolean => "ReadBoolean()",
            SpecialType.System_SByte => "ReadSByte()",
            SpecialType.System_Byte => "ReadByte()",
            SpecialType.System_Char => "ReadChar()",
            SpecialType.System_DateTime => "ReadDateTime()",
            SpecialType.System_Double => "ReadDouble()",
            SpecialType.System_Single => "ReadSingle()",
            SpecialType.System_String => "ReadString()",
            SpecialType.System_Int16 => "ReadInt16()",
            SpecialType.System_Int32 => "ReadInt32()",
            SpecialType.System_Int64 => "ReadInt64()",
            SpecialType.System_UInt16 => "ReadUInt16()",
            SpecialType.System_UInt32 => "ReadUInt32()",
            SpecialType.System_UInt64 => "ReadUInt64()",
            _ => ""
        };
}