using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Vogen;

internal class WriteBsonSerializers
{
    public static void WriteIfNeeded(SourceProductionContext context, Compilation compilation, List<VoWorkItem> workItems)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }

        var items = workItems.Where(i => i.Config.Conversions.HasFlag(Conversions.Bson)).ToList();

        var toWrite = items.Select(GenerateSerializerReadyForWriting).ToList();
        
        foreach (var eachToWrite in toWrite)
        {
            WriteSerializer(eachToWrite, context);
        }
        
        WriteRegisterer(toWrite, compilation, context);
    }

    private static void WriteSerializer(SerializerEntry eachToWrite, SourceProductionContext context)
    {
        SourceText sourceText = SourceText.From(eachToWrite.SourceCode, Encoding.UTF8);

        Util.TryWriteUsingUniqueFilename(eachToWrite.Filename, context, sourceText);
    }

    private static void WriteRegisterer(List<SerializerEntry> items, Compilation compilation, SourceProductionContext context)
    {
        if (items.Count == 0)
        {
            return;
        }

        var assemblyName = compilation.AssemblyName?.Replace(".", "_") ?? "";
        if (assemblyName.EndsWith("_dll") || assemblyName.EndsWith("_exe"))
        {
            assemblyName = assemblyName[..^4];
        }

        string className = $"BsonSerializationRegister";
        if(assemblyName.Length > 0)
        {
            className = className + "For" + assemblyName;
        }

        string source =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              public static class {{className}}
              {
                    static {{className}}()
                    {
                        {{TextForEachRegisterCall(items)}}
                    }
                    
                    public static void TryRegister() { }
              }
              """;
        
        SourceText sourceText = SourceText.From(source, Encoding.UTF8);

        Util.TryWriteUsingUniqueFilename(className, context, sourceText);
        
    }

    private static string TextForEachRegisterCall(List<SerializerEntry> items)
    {
        StringBuilder sb = new();
        foreach (SerializerEntry eachEntry in items)
        {
            sb.AppendLine($"global::MongoDB.Bson.Serialization.BsonSerializer.TryRegisterSerializer(new {eachEntry.SerializerFullyQualifiedName}());");
        }

        return sb.ToString();
    }

    public record SerializerEntry(string SerializerFullyQualifiedName, string Filename, string SourceCode);

    private static SerializerEntry GenerateSerializerReadyForWriting(VoWorkItem spec)
    {
        var fullNamespace = spec.FullNamespace;

        var isPublic = spec.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        string wrapperName = Util.EscapeIfRequired(spec.WrapperType.Name);
        string underlyingTypeName = spec.UnderlyingTypeFullName;
        
        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              {{ns}}
                  
              {{accessor}} partial class {{wrapperName}}BsonSerializer : global::MongoDB.Bson.Serialization.Serializers.SerializerBase<{{wrapperName}}>
              {
                  private readonly global::MongoDB.Bson.Serialization.IBsonSerializer<{{underlyingTypeName}}> _serializer = global::MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer<{{underlyingTypeName}}>();
              
                  public override {{wrapperName}} Deserialize(global::MongoDB.Bson.Serialization.BsonDeserializationContext context, global::MongoDB.Bson.Serialization.BsonDeserializationArgs args) => 
                    Deserialize(_serializer.Deserialize(context, args));
              
                  public override void Serialize(global::MongoDB.Bson.Serialization.BsonSerializationContext context, global::MongoDB.Bson.Serialization.BsonSerializationArgs args, {{wrapperName}} value) => 
                    _serializer.Serialize(context, args, value.Value);
                    
                static {{wrapperName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                
                [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
                    
              }
              """;
        
        var fn = string.IsNullOrEmpty(fullNamespace) ? "" : fullNamespace + ".";
        string serializerFqn = $"{fn}{wrapperName}BsonSerializer";
        
        var unsanitized = $"{spec.WrapperType.ToDisplayString()}_bson.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new SerializerEntry(serializerFqn, filename, sb);
    }
}