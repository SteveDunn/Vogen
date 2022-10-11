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
            IncrementalValueProvider<(ImmutableArray<VoTarget> Left, ImmutableArray<AttributeSyntax> Right)> targets = GetTargets(context);

            IncrementalValueProvider<(Compilation Left, (ImmutableArray<VoTarget> Left, ImmutableArray<AttributeSyntax> Right) Right)> compilationAndValues
                = context.CompilationProvider.Combine(targets);

            context.RegisterSourceOutput(compilationAndValues,
                static (spc, source) => Execute(
                    source.Left, 
                    source.Right.Left, 
                    source.Right.Right,
                    spc));
        }

        private static IncrementalValueProvider<(ImmutableArray<VoTarget> Left, ImmutableArray<AttributeSyntax> Right)> GetTargets(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<VoTarget> voFilter = context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => VoFilter.IsTarget(s),
                    transform: static (ctx, _) => VoFilter.TryGetTarget(ctx))
                .Where(static m => m is not null)!;

            IncrementalValuesProvider<AttributeSyntax> globalConfigFilter = context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => GlobalConfigFilter.IsTarget(s),
                    transform: static (ctx, _) => GlobalConfigFilter.GetAssemblyLevelAttributeForConfiguration(ctx))
                .Where(static m => m is not null)!;

            IncrementalValueProvider<(ImmutableArray<VoTarget> Left, ImmutableArray<AttributeSyntax> Right)> targetsAndDefaultAttributes
                = voFilter.Collect().Combine(globalConfigFilter.Collect());

            return targetsAndDefaultAttributes;
        }

        static void Execute(
            Compilation compilation, 
            ImmutableArray<VoTarget> typeDeclarations,
            ImmutableArray<AttributeSyntax> globalConfigAttributes,
            SourceProductionContext context)
        {
            if (typeDeclarations.IsDefaultOrEmpty)
            {
                return;
            }
            
            // if there are some, get the
            var buildResult =
                GlobalConfigFilter.GetDefaultConfigFromGlobalAttribute(globalConfigAttributes, compilation);
            
            foreach (var diagnostic in buildResult.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            VogenConfiguration? globalConfig = buildResult.ResultingConfiguration;

            // get all of the ValueObject types found.
            List<VoWorkItem> workItems = GetWorkItems(typeDeclarations, context, globalConfig).ToList();

            if (workItems.Count > 0)
            {
                foreach (var eachWorkItem in workItems)
                {
                    WriteWorkItems.WriteVo(eachWorkItem, context);
                }
            }
        }

        static IEnumerable<VoWorkItem> GetWorkItems(ImmutableArray<VoTarget> targets,
            SourceProductionContext context,
            VogenConfiguration? globalConfig)
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
                
                var ret = BuildWorkItems.TryBuild(eachTarget, context, globalConfig);
                
                if (ret is not null)
                {
                    yield return ret;
                }
            }
        }
    }
}