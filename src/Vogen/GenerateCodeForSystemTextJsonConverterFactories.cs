using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Types;

namespace Vogen;

internal class GenerateCodeForSystemTextJsonConverterFactories
{
    public static void WriteIfNeeded(VogenConfiguration? globalConfig,
        List<VoWorkItem> workItems,
        SourceProductionContext context,
        Compilation compilation,
        VogenKnownSymbols vogenKnownSymbols)
    {
        if (vogenKnownSymbols.JsonConverterFactory is null)
        {
            return;
        }
        
        var generation = globalConfig?.SystemTextJsonConverterFactoryGeneration ??
                         VogenConfiguration.DefaultInstance.SystemTextJsonConverterFactoryGeneration;

        if (generation == SystemTextJsonConverterFactoryGeneration.Omit)
        {
            return;
        }
        
        
        var entries = workItems.Where(i => i.Config.Conversions.HasFlag(Conversions.SystemTextJson)).Select(BuildEntry);
        
        var fullNamespace = ProjectName.FromAssemblyName(compilation.Assembly.Name);

        var ns = $"namespace {fullNamespace};";
        
        string source =
            $$"""
            
            {{GeneratedCodeSegments.Preamble}}
            
            {{ns}}
            
            public class VogenTypesFactory : global::System.Text.Json.Serialization.JsonConverterFactory
            {
                public VogenTypesFactory() { }
                
                private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>> _lookup = 
                    new global::System.Collections.Generic.Dictionary<global::System.Type, global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>> {
                            {{string.Join(",", entries)}}
                    };
                
                public override bool CanConvert(global::System.Type typeToConvert) => _lookup.ContainsKey(typeToConvert);
                
                public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) =>
                    _lookup[typeToConvert].Value;
            }
            """;

        context.AddSource("SystemTextJsonConverterFactory_g.cs", Util.FormatSource(source));
    }

    private static string BuildEntry(VoWorkItem eachStj)
    {
        var fqn = string.IsNullOrEmpty(eachStj.FullNamespace)
            ? $"{eachStj.VoTypeName}"
            : $"global::{eachStj.FullNamespace}.{eachStj.VoTypeName}";
            
        return $$"""{ typeof({{fqn}}), new global::System.Lazy<global::System.Text.Json.Serialization.JsonConverter>(() => new {{fqn}}.{{eachStj.VoTypeName}}SystemTextJsonConverter()) }""";
    }
}