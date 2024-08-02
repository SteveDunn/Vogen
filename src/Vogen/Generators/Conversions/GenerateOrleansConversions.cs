using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

public class GenerateOrleansConversions : IGenerateConversion, IGenerateAssemblyAttributes
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item) => string.Empty;

    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.Orleans))
        {
            return string.Empty;
        }

        return $$"""
                 internal sealed class OrleansSerializer : global::Orleans.Serialization.Codecs.IFieldCodec<{{item.VoTypeName}}>
                 {
                     public void WriteField<TBufferWriter>(ref global::Orleans.Serialization.Buffers.Writer<TBufferWriter> writer, uint fieldIdDelta, System.Type expectedType, {{item.VoTypeName}} value)
                         where TBufferWriter : global::System.Buffers.IBufferWriter<byte>
                     {
                         var baseCodec = writer.Session.CodecProvider.GetCodec<{{item.UnderlyingTypeFullName}}>();
                         baseCodec.WriteField(ref writer, fieldIdDelta, typeof({{item.UnderlyingTypeFullName}}), value.Value);
                     }
                 
                     public {{item.VoTypeName}} ReadValue<TInput>(ref global::Orleans.Serialization.Buffers.Reader<TInput> reader, global::Orleans.Serialization.WireProtocol.Field field)
                     {
                         var baseCodec = reader.Session.CodecProvider.GetCodec<{{item.UnderlyingTypeFullName}}>();
                         var baseValue = baseCodec.ReadValue(ref reader, field);
                         return {{item.VoTypeName}}.From(baseValue);
                     }
                 }

                 internal sealed class OrleansCopier : global::Orleans.Serialization.Cloning.IDeepCopier<{{item.VoTypeName}}>
                 {
                     public {{item.VoTypeName}} DeepCopy({{item.VoTypeName}} input, global::Orleans.Serialization.Cloning.CopyContext context)
                     {
                         return {{item.VoTypeName}}.From(input.Value);
                     }
                 }
                 
                 internal class OrleansProvider : global::Orleans.Serialization.Configuration.TypeManifestProviderBase
                 {
                     protected override void ConfigureInner(global::Orleans.Serialization.Configuration.TypeManifestOptions config)
                     {
                         config.Serializers.Add(typeof(OrleansSerializer));
                         config.Copiers.Add(typeof(OrleansCopier));
                     }
                 }

                 """;
    }

    public string GenerateAssemblyAttributes(TypeDeclarationSyntax tds, VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.Orleans))
        {
            return string.Empty;
        }

        var voNamespace = item.FullNamespace;

        return string.IsNullOrWhiteSpace(voNamespace)
            ? $"[assembly: global::Orleans.Serialization.Configuration.TypeManifestProvider({item.VoTypeName}.OrleansProvider))]"
            : $"[assembly: global::Orleans.Serialization.Configuration.TypeManifestProvider(typeof({voNamespace}.{item.VoTypeName}.OrleansProvider))]";
    }
}