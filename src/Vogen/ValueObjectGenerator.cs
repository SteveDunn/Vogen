using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Diagnostics;

namespace Vogen
{
    [Generator]
    public class ValueObjectGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<VoTarget?> targets = GetTargets(context);

            IncrementalValueProvider<(Compilation, ImmutableArray<VoTarget?>)> compilationAndTypes
                = context.CompilationProvider.Combine(targets.Collect());
            
            context.RegisterSourceOutput(compilationAndTypes,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static IncrementalValuesProvider<VoTarget?> GetTargets(IncrementalGeneratorInitializationContext context) =>
            context.SyntaxProvider.CreateSyntaxProvider(
                    predicate: static (s, _) => VoFilter.IsTarget(s),
                    transform: static (ctx, _) => VoFilter.TryGetTarget(ctx))
                .Where(static m => m is not null);

        static void Execute(Compilation compilation, ImmutableArray<VoTarget?> typeDeclarations,
            SourceProductionContext context)
        {
            if (typeDeclarations.IsDefaultOrEmpty)
            {
                return;
            }

            DiagnosticCollection diagnostics = new DiagnosticCollection();

            List<VoWorkItem> workItems = GetWorkItems(typeDeclarations, context, diagnostics).ToList();

            foreach (var eachWorkItem in workItems)
            {
                WriteWorkItems.WriteVo(eachWorkItem, compilation, context, diagnostics);
            }

            ReportErrors(context, diagnostics);
        }

        static IEnumerable<VoWorkItem> GetWorkItems(
            ImmutableArray<VoTarget?> targets,
            SourceProductionContext context,
            DiagnosticCollection diagnostics)
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
                
                var ret = BuildWorkItems.TryBuild(eachTarget, context, diagnostics);
                
                if (ret is not null)
                {
                    yield return ret;
                }
            }
        }

        private static void ReportErrors(SourceProductionContext context,
            DiagnosticCollection syntaxReceiverDiagnosticMessages)
        {
            foreach (var eachDiag in syntaxReceiverDiagnosticMessages)
            {
                context.ReportDiagnostic(eachDiag);
            }
        }
    }
}