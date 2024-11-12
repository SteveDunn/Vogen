using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Vogen;

internal class GenerateCodeForMessagePack
{
    public static void GenerateForAMarkerClass(SourceProductionContext context, Compilation compilation, MarkerClassDefinition markerClass)
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
                          {{GenerateSource("public", eachMarker.Marker!.VoSymbol, eachMarker.Marker.UnderlyingTypeSymbol)}}
                      """);
            }

            return sb.ToString();
        }
    }

    public static void GenerateForApplicableValueObjects(SourceProductionContext context,
        Compilation compilation,
        List<VoWorkItem> valueObjects)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
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

            Util.TryWriteUsingUniqueFilename(eachToWrite.Filename, context, sourceText);
        }
    }

    public record FormatterSourceAndFilename(string FormatterFullyQualifiedName, string Filename, string SourceCode);

    private static FormatterSourceAndFilename GenerateSourceAndFilename(
        string accessibility,
        INamedTypeSymbol wrapperSymbol,
        string theNamespace,
        INamedTypeSymbol underlyingSymbol)
    {
        string wrapperName = Util.EscapeIfRequired(wrapperSymbol.Name);

        var ns = string.IsNullOrEmpty(theNamespace) ? string.Empty : $"namespace {theNamespace};";

        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              {{ns}}

              {{GenerateSource(accessibility, wrapperSymbol, underlyingSymbol)}}          
              """;

        var fn = string.IsNullOrEmpty(theNamespace) ? "" : theNamespace + ".";
        string serializerFqn = $"{fn}{wrapperName}MessagePackFormatter";

        var unsanitized = $"{wrapperSymbol.ToDisplayString()}_messagepack.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new FormatterSourceAndFilename(serializerFqn, filename, sb);
    }


    private static string GenerateSource(string accessibility, INamedTypeSymbol wrapperSymbol, INamedTypeSymbol underlyingSymbol)
    {
        var accessor = accessibility;

        string wrapperNameShort = Util.EscapeIfRequired(wrapperSymbol.Name);
        string wrapperName = Util.EscapeIfRequired(wrapperSymbol.FullName() ?? wrapperSymbol.Name);

        string underlyingTypeName = underlyingSymbol.FullName() ?? wrapperSymbol.Name;
        
        string readMethod = GenerateReadMethod();

        if (readMethod.Length == 0)
        {
            return "#error unsupported underlying type " + underlyingSymbol.SpecialType;
        }

        string sb =
            $$"""
              {{accessor}} partial class {{wrapperNameShort}}MessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{{wrapperName}}>
              {
                  public void Serialize(ref global::MessagePack.MessagePackWriter writer, {{wrapperName}} value, global::MessagePack.MessagePackSerializerOptions options) =>
                      writer.Write(value.Value);
              
                  public {{wrapperName}} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options) =>
                      Deserialize(reader.{{readMethod}});
              
                static {{wrapperName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                
                [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
              }
              """;

        return sb;

        string GenerateReadMethod()
        {
            if(underlyingSymbol.SpecialType == SpecialType.System_Boolean)
                return "ReadBoolean()";
            if(underlyingSymbol.SpecialType == SpecialType.System_SByte)
                return "ReadSByte()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Byte)
                return "ReadByte()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Char)
                return "ReadChar()";
            if(underlyingSymbol.SpecialType == SpecialType.System_DateTime)
                return "ReadDateTime()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Double)
                return "ReadDouble()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Single)
                return "ReadSingle()";
            if(underlyingSymbol.SpecialType == SpecialType.System_String)
                return "ReadString()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Int16)
                return "ReadInt16()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Int32)
                return "ReadInt32()";
            if(underlyingSymbol.SpecialType == SpecialType.System_Int64)
                return "ReadInt64()";
            if(underlyingSymbol.SpecialType == SpecialType.System_UInt16)
                return "ReadUInt16()";
            if(underlyingSymbol.SpecialType == SpecialType.System_UInt32)
                return "ReadUInt32()";
            if(underlyingSymbol.SpecialType == SpecialType.System_UInt64)
                return "ReadUInt64()";
            return "";
        }
    }
}