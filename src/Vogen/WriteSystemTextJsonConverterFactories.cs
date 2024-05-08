using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class WriteSystemTextJsonConverterFactories
{
    public static void WriteIfNeeded(VogenConfiguration? globalConfig, List<VoWorkItem> workItems, SourceProductionContext context)
    {
        var generation = globalConfig?.SystemTextJsonConverterFactoryGeneration ??
                         VogenConfiguration.DefaultInstance.SystemTextJsonConverterFactoryGeneration;

        if (generation == SystemTextJsonConverterFactoryGeneration.Omit)
        {
            return;
        }
        
        var stjs = workItems.Where(i => i.Conversions.HasFlag(Conversions.SystemTextJson)).Select(BuildEntry);

        string s2 =
            $$"""
            
            #nullable enable annotations
            #nullable disable warnings
            
            {{GeneratedCodeSegments.Preamble}}
            
            public class VogenTypesFactory : global::System.Text.Json.Serialization.JsonConverterFactory
            {
                public VogenTypesFactory() { }
                private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>> _lookup = 
                    new global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>>(
                        new global::System.Collections.Generic.KeyValuePair<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>>[] {
                            {{string.Join(",", stjs)}}
                    });
                
                public override bool CanConvert(global::System.Type typeToConvert) => _lookup.ContainsKey(typeToConvert);
                
                public override global::System.Text.Json.Serialization.JsonConverter? CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) =>
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
            
        return $$"""new(typeof({{fqn}}), new(() => new {{fqn}}.{{eachStj.VoTypeName}}SystemTextJsonConverter()))""";
    }
}