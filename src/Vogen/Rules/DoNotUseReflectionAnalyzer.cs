using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Vogen.Diagnostics;

namespace Vogen.Rules;

// An analyzer that stops: `CustomerId = Activator.CreateInstance<CustomerId>();` and `CustomerId = Activator.CreateInstance(typeof(CustomerId))`.
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseReflectionAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseReflection,
        "Using Reflection to create Value Objects is prohibited",
        "Type '{0}' cannot be constructed via Reflection as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Do not use Reflection to create Value Objects.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compStartCtx =>
        {
            INamedTypeSymbol? activator = compStartCtx.Compilation.GetTypeByMetadataName("System.Activator");
            if (activator is null) return;

            var methods = activator.GetMembers("CreateInstance").OfType<IMethodSymbol>().ToList();
            var nonGeneric = methods.FirstOrDefault(m => m.Parameters.Length == 1 && !m.IsGenericMethod);
            var generic = methods.FirstOrDefault(m => m.Parameters.Length == 0 && m.IsGenericMethod);

            compStartCtx.RegisterOperationAction(operationCtx =>
            {
                var invocation = (IInvocationOperation) operationCtx.Operation;

                var methodSymbol = invocation.TargetMethod;

                if (SymbolEqualityComparer.Default.Equals(methodSymbol.OriginalDefinition, generic?.OriginalDefinition))
                {
                    ReportIfNeeded(methodSymbol.ReturnType, operationCtx, invocation.Syntax.GetLocation());
                    return;
                }

                if (SymbolEqualityComparer.Default.Equals(methodSymbol, nonGeneric))
                {
                    var op = invocation.Arguments[0].Value as ITypeOfOperation;
                    var typeSymbol = op?.TypeOperand;

                    ReportIfNeeded(typeSymbol, operationCtx, invocation.Syntax.GetLocation());
                }
            }, OperationKind.Invocation);
        });
    }

    private static void ReportIfNeeded(ISymbol? typeInfo, OperationAnalysisContext ctx, Location location)
    {
        if (typeInfo is not INamedTypeSymbol symbol) return;

        if (!VoFilter.IsTarget(symbol)) return;

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, symbol.Name, location);

        ctx.ReportDiagnostic(diagnostic);
    }
}