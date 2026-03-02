using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generators.Conversions;

namespace Vogen;

internal class GenerateCodeForMessagePack : IGenerateConversion
{
    public static void GenerateForMarkerClasses(SourceProductionContext context, ImmutableArray<MarkerClassDefinition> conversionMarkerClasses)
    {
        foreach (MarkerClassDefinition eachMarkerClass in conversionMarkerClasses)
        {
            GenerateForAMarkerClass(context, eachMarkerClass);
        }
    }

    private static void GenerateForAMarkerClass(SourceProductionContext context, MarkerClassDefinition markerClass)
    {
        var markerClassSymbol = markerClass.MarkerClassSymbol;

        if (!markerClass.AttributeDefinitions.Any(m => m.Marker?.Kind is ConversionMarkerKind.MessagePack))
        {
            return;
        }

        string pns = markerClassSymbol.FullUnalisaedNamespace();

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

        string filename = Util.GetLegalFilenameForMarkerClass(markerClass.MarkerClassSymbol, ConversionMarkerKind.MessagePack);

        Util.AddSourceToContext(filename, context, sourceText);

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
                        if (x.Marker is null)
                        {
                            return null;
                        }

                        string wrapperNameShort = x.Marker.VoSymbol.Name;
                        
                        return $"new {wrapperNameShort}MessagePackFormatter()";
                    }).ToArray();

                return string.Join(", ", names);
            }
        }

        string GenerateFormatters()
        {
            StringBuilder sb = new();

            foreach (MarkerAttributeDefinition eachAttr in markerClass.AttributeDefinitions.Where(
                         m => m.Marker?.Kind is ConversionMarkerKind.MessagePack))
            {
                sb.AppendLine(
                    $$"""
                          {{GenerateSource("public", eachAttr.Marker!.VoSymbol, eachAttr.Marker.UnderlyingTypeSymbol)}}
                      """);
            }

            return sb.ToString();
        }
    }

    public static void GenerateForApplicableValueObjects(SourceProductionContext context,
        Compilation compilation,
        List<VoWorkItem> valueObjects)
    {
        if (!compilation.IsAtLeastCSharp12())
        {
            return;
        }

        var matchingVos = valueObjects.Where(i => i.Config.Conversions.HasFlag(Conversions.MessagePack)).ToList();

        List<MessagePackStandalone> items = matchingVos.Select(MessagePackStandalone.FromWorkItem).ToList();

        List<FormatterSourceAndFilename> toWrite = items.Select(
            p => GenerateSourceAndFilename(p.WrapperAccessibility, p.WrapperType, p.ContainerNamespace, p.UnderlyingType)).ToList();

        foreach (var eachToWrite in toWrite)
        {
            SourceText sourceText = Util.FormatSource(eachToWrite.SourceCode);

            Util.AddSourceToContext(eachToWrite.Filename, context, sourceText);
        }
    }

    private record FormatterSourceAndFilename(string Filename, string SourceCode);

    private static FormatterSourceAndFilename GenerateSourceAndFilename(string accessibility,
        INamedTypeSymbol wrapperSymbol,
        string theNamespace,
        INamedTypeSymbol underlyingSymbol)
    {
        var ns = string.IsNullOrEmpty(theNamespace) ? string.Empty : $"namespace {theNamespace};";

        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              {{ns}}

              {{GenerateSource(accessibility, wrapperSymbol, underlyingSymbol)}}          
              """;

        var unsanitized = $"{wrapperSymbol.ToDisplayString()}_messagepack.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new FormatterSourceAndFilename(filename, sb);
    }

    private static string GenerateSource(string accessibility,
        INamedTypeSymbol wrapperSymbol,
        INamedTypeSymbol underlyingSymbol)
    {
        var accessor = accessibility;

        string wrapperNameShort = Util.EscapeKeywordsIfRequired(wrapperSymbol.Name);
        string wrapperName = wrapperSymbol.EscapedFullName();

        string underlyingTypeName = underlyingSymbol.EscapedFullName();
        
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
    
    /// <summary>
    /// Represents a marker for a MessagePack conversion
    /// </summary>
    /// <param name="ContainerNamespace">The namespace of the containing marker class.</param>
    /// <param name="WrapperType">The symbol for the value object wrapper class.</param>
    /// <param name="UnderlyingType">The symbol for the underlying primitive type.</param>
    /// <param name="WrapperAccessibility">The accessibility (public, internal, etc.) of the generated type.</param>
    private record MessagePackStandalone(
        string ContainerNamespace,
        INamedTypeSymbol WrapperType, 
        INamedTypeSymbol UnderlyingType, 
        string WrapperAccessibility)
    {
        public static MessagePackStandalone FromWorkItem(VoWorkItem voWorkItem)
        {
            var isPublic = voWorkItem.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
            var accessor = isPublic ? "public" : "internal";

            return new(voWorkItem.FullUnaliasedNamespace, voWorkItem.WrapperType, voWorkItem.UnderlyingType, accessor);
        }
    }

    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols)
    {
        if (!item.HasConversion(Conversions.MessagePack))
        {
            return string.Empty;
        }

        string fqName = $"{item.WrapperType.EscapedFullName()}MessagePackFormatter";

        return $"[global::MessagePack.MessagePackFormatter(typeof({fqName}))]";
    }

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols) => "";
}