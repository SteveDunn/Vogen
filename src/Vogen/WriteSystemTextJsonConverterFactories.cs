using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class WriteSystemTextJsonConverterFactories
{
    public static void WriteIfNeeded(VogenConfiguration? globalConfig,
        List<VoWorkItem> workItems,
        SourceProductionContext context,
        Compilation compilation)
    {
        if (compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonConverterFactory") is null)
        {
            return;
        }
        
        var generation = globalConfig?.SystemTextJsonConverterFactoryGeneration ??
                         VogenConfiguration.DefaultInstance.SystemTextJsonConverterFactoryGeneration;

        if (generation == SystemTextJsonConverterFactoryGeneration.Omit)
        {
            return;
        }
        
        var stjs = workItems.Where(i => i.Config.Conversions.HasFlag(Conversions.SystemTextJson)).Select(BuildEntry);

        string s2 =
            $$"""
            
            {{GeneratedCodeSegments.Preamble}}
            
            public class VogenTypesFactory : global::System.Text.Json.Serialization.JsonConverterFactory
            {
                public VogenTypesFactory() { }
                private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>> _lookup = 
                    new global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>> {
                            {{string.Join(",", stjs)}}
                    };
                
                public override bool CanConvert(global::System.Type typeToConvert) => _lookup.ContainsKey(typeToConvert);
                
                public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) =>
                    _lookup[typeToConvert].Value;
            }
            """;

        context.AddSource("SystemTextJsonConverterFactory_g.cs", s2);
    }

    private static string BuildEntry(VoWorkItem eachStj)
    {
        var fqn = string.IsNullOrEmpty(eachStj.FullNamespace)
            ? $"{eachStj.VoTypeName}"
            : $"{eachStj.FullNamespace}.{eachStj.VoTypeName}";
            
        return $$"""{ typeof({{fqn}}), new global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>(() => new {{fqn}}.{{eachStj.VoTypeName}}SystemTextJsonConverter()) }""";
    }
}