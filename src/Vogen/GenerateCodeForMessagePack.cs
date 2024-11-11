using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Vogen;

public record MessagePackGeneratorParams(
    string Namespace, 
    INamedTypeSymbol WrapperType, 
    INamedTypeSymbol UnderlyingType, 
    string WrapperAccessibility)
{
    public static MessagePackGeneratorParams FromWorkItem(VoWorkItem voWorkItem)
    {
        var isPublic = voWorkItem.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        return new(voWorkItem.FullNamespace, voWorkItem.WrapperType, voWorkItem.UnderlyingType, accessor);
    }
}


internal class GenerateCodeForMessagePack
{
    public static void Generate(SourceProductionContext context, Compilation compilation, List<MessagePackGeneratorParams> items)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }

        List<FormatterSourceAndFilename> toWrite = items.Select(GenerateSourceAndFilename).ToList();
        
        foreach (var eachToWrite in toWrite)
        {
            SourceText sourceText = SourceText.From(eachToWrite.SourceCode, Encoding.UTF8);

            Util.TryWriteUsingUniqueFilename(eachToWrite.Filename, context, sourceText);
        }
    }

    public static void GenerateForApplicableWorkItems(SourceProductionContext context, Compilation compilation, List<VoWorkItem> workItems)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }

        var items = workItems.Where(i => i.Config.Conversions.HasFlag(Conversions.MessagePack)).ToList();

        var toWrite = items.Select(MessagePackGeneratorParams.FromWorkItem).ToList();
        
        Generate(context, compilation, toWrite);
    }

    public record FormatterSourceAndFilename(string FormatterFullyQualifiedName, string Filename, string SourceCode);

    private static FormatterSourceAndFilename GenerateSourceAndFilename(MessagePackGeneratorParams p)
    {
        var fullNamespace = p.Namespace;

        var accessor = p.WrapperAccessibility;

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        string wrapperName = Util.EscapeIfRequired(p.WrapperType.Name);

        string underlyingTypeName = p.UnderlyingType.FullName() ?? p.WrapperType.Name;

        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              {{ns}}
                  
              {{accessor}} partial class {{wrapperName}}MessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{{wrapperName}}>
              {
                  public void Serialize(ref global::MessagePack.MessagePackWriter writer, {{wrapperName}} value, global::MessagePack.MessagePackSerializerOptions options) =>
                      writer.Write(value.Value);
              
                  public {{wrapperName}} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options) =>
                      Deserialize(reader.{{GenerateReadMethod()}});
              
                static {{wrapperName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                
                [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
              }
              """;
        
        var fn = string.IsNullOrEmpty(fullNamespace) ? "" : fullNamespace + ".";
        string serializerFqn = $"{fn}{wrapperName}MessagePackFormatter";
        
        var unsanitized = $"{p.WrapperType.ToDisplayString()}_messagepack.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new FormatterSourceAndFilename(serializerFqn, filename, sb);
    }

    private static string GenerateReadMethod()
    {
        return "ReadInt32()";
    }
}