using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Vogen;

internal class GenerateCodeForOrleansSerializers
{
    public static void WriteIfNeeded(SourceProductionContext context, List<VoWorkItem> workItems)
    {
        var items = workItems.Where(i => i.Config.Conversions.HasFlag(Conversions.Orleans)).ToList();

        var toWrite = items.Select(GenerateSource).ToList();
        
        foreach (var eachEntry in toWrite)
        {
            WriteSerializer(eachEntry, context);
        }
    }

    private static void WriteSerializer(SerializerEntry entry, SourceProductionContext context)
    {
        SourceText sourceText = SourceText.From(entry.SourceCode, Encoding.UTF8);

        Util.TryWriteUsingUniqueFilename(entry.Filename, context, sourceText);
    }

    public record SerializerEntry(string Filename, string SourceCode);

    private static SerializerEntry GenerateSource(VoWorkItem item)
    {
        var fullNamespace = item.FullUnaliasedNamespace;

        var isPublic = item.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        string unescapedWrapperName = item.WrapperType.Name;
        string wrapperName = Util.EscapeKeywordsIfRequired(unescapedWrapperName);
        string underlyingTypeName = item.UnderlyingTypeFullNameWithGlobalAlias;

        var assemblyAttribute = string.IsNullOrWhiteSpace(fullNamespace)
            ? $"[assembly: global::Orleans.Serialization.Configuration.TypeManifestProvider({item.VoTypeName}OrleansProvider))]"
            : $"[assembly: global::Orleans.Serialization.Configuration.TypeManifestProvider(typeof({fullNamespace}.{item.VoTypeName}OrleansProvider))]";

        
        string sb =
            $$"""
              {{GeneratedCodeSegments.Preamble}}
              
              {{assemblyAttribute}}

              {{ns}}
                  
              {{accessor}} sealed class {{unescapedWrapperName}}OrleansSerializer : global::Orleans.Serialization.Codecs.IFieldCodec<{{item.VoTypeName}}>
              {
                  public void WriteField<TBufferWriter>(ref global::Orleans.Serialization.Buffers.Writer<TBufferWriter> writer, uint fieldIdDelta, System.Type expectedType, {{item.VoTypeName}} value)
                      where TBufferWriter : global::System.Buffers.IBufferWriter<byte>
                  {
                      var baseCodec = writer.Session.CodecProvider.GetCodec<{{underlyingTypeName}}>();
                      baseCodec.WriteField(ref writer, fieldIdDelta, typeof({{underlyingTypeName}}), value.Value);
                  }
              
                  public {{item.VoTypeName}} ReadValue<TInput>(ref global::Orleans.Serialization.Buffers.Reader<TInput> reader, global::Orleans.Serialization.WireProtocol.Field field)
                  {
                      var baseCodec = reader.Session.CodecProvider.GetCodec<{{underlyingTypeName}}>();
                      var baseValue = baseCodec.ReadValue(ref reader, field);
                      return UnsafeDeserialize(default, baseValue);
                  }
                  
                  [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                  static extern {{wrapperName}} UnsafeDeserialize({{wrapperName}} @this, {{underlyingTypeName}} value);      
                  
              }
              
              {{accessor}} sealed class {{unescapedWrapperName}}OrleansCopier : global::Orleans.Serialization.Cloning.IDeepCopier<{{item.VoTypeName}}>
              {
                  public {{item.VoTypeName}} DeepCopy({{item.VoTypeName}} input, global::Orleans.Serialization.Cloning.CopyContext context)
                  {
                      {{GetNullCheck(item)}}
                      return {{item.VoTypeName}}.From(input.Value);
                  }
              }
              
              {{accessor}} class {{unescapedWrapperName}}OrleansProvider : global::Orleans.Serialization.Configuration.TypeManifestProviderBase
              {
                  protected override void ConfigureInner(global::Orleans.Serialization.Configuration.TypeManifestOptions config)
                  {
                      config.Serializers.Add(typeof({{unescapedWrapperName}}OrleansSerializer));
                      config.Copiers.Add(typeof({{unescapedWrapperName}}OrleansCopier));
                  }
              }
                    
              """;
        
        var unsanitized = $"{item.WrapperType.ToDisplayString()}_orleans.g.cs";
        string filename = Util.SanitizeToALegalFilename(unsanitized);
        return new SerializerEntry(filename, sb);
    }

    private static string GetNullCheck(VoWorkItem item) => item.IsTheWrapperAReferenceType ? "if (input is null) return null;" : "";
}