using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen
{
    [Generator]
    public class ValueObjectGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {   
            Found found = GetTargets(context);

            IncrementalValueProvider<ImmutableArray<VoTarget>> collectedVos = found.vos.Collect();
            
            var targetsAndConfig = collectedVos.Combine(found.globalConfig.Collect());

            IncrementalValueProvider<(Compilation Compilation, (ImmutableArray<VoTarget> Targets, ImmutableArray<VogenConfigurationBuildResult>
                GlobalConfigSyntax) Found)> compilationAndValues = context.CompilationProvider.Combine(targetsAndConfig);
            
            context.RegisterSourceOutput(compilationAndValues,
                static (spc, source) => Execute(
                    source.Compilation, 
                    source.Found.Targets, 
                    source.Found.GlobalConfigSyntax,
                    spc));
        }

        private static Found GetTargets(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<VoTarget> targets = context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (node, _) => VoFilter.IsTarget(node),
                    transform: static (ctx, _) => VoFilter.TryGetTarget(ctx))
                .Where(static m => m is not null)!;

            IncrementalValuesProvider<VogenConfigurationBuildResult> globalConfig = context.SyntaxProvider.ForAttributeWithMetadataName(
                    "Vogen.VogenDefaultsAttribute",
                    predicate: (node, _) => node is CompilationUnitSyntax,
                    transform: (ctx, _) => ManageAttributes.GetDefaultConfigFromGlobalAttribute(ctx))
                .Where(static m => m is not null)!;

            return new Found(targets, globalConfig);
        }

        record struct Found(
            IncrementalValuesProvider<VoTarget> vos,
            IncrementalValuesProvider<VogenConfigurationBuildResult> globalConfig);
    
        static void Execute(
            Compilation compilation, 
            ImmutableArray<VoTarget> targets,
            ImmutableArray<VogenConfigurationBuildResult> globalConfigBuildResult,
            SourceProductionContext context)
        {
            if (targets.IsDefaultOrEmpty)
            {
                return;
            }

            // if there are some, get the default global config
            VogenConfigurationBuildResult buildResult = globalConfigBuildResult.IsDefaultOrEmpty
                ? VogenConfigurationBuildResult.Null
                : globalConfigBuildResult.ElementAt(0);
            
            foreach (var diagnostic in buildResult.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            VogenConfiguration? globalConfig = buildResult.ResultingConfiguration;

            // get all the ValueObject types found.
            List<VoWorkItem> workItems = GetWorkItems(targets, context, globalConfig, compilation).ToList();
            
            WriteSwashbuckleSchemaRelatedCode.WriteIfNeeded(globalConfig, context, compilation, workItems);

            if (workItems.Count > 0)
            {
                var mergedConfig = CombineConfigurations.CombineAndResolveAnyGlobalConfig(globalConfig);
                WriteStaticAbstracts.WriteIfNeeded(mergedConfig, context, compilation);

                WriteSystemTextJsonConverterFactories.WriteIfNeeded(mergedConfig, workItems, context, compilation);

                foreach (var eachWorkItem in workItems)
                {
                    WriteWorkItems.WriteVo(eachWorkItem, context);
                }
            }
        }
        
        private static IEnumerable<VoWorkItem> GetWorkItems(ImmutableArray<VoTarget> targets,
            SourceProductionContext context,
            VogenConfiguration? globalConfig,
            Compilation compilation)
        {
            if (targets.IsDefaultOrEmpty)
            {
                yield break;
            }

            foreach (var eachTarget in targets)
            {
                if (eachTarget is null)
                {
                    continue;
                }
                
                var ret = BuildWorkItems.TryBuild(eachTarget, context, globalConfig, compilation);
                
                if (ret is not null)
                {
                    yield return ret;
                }
            }
        }
    }
}