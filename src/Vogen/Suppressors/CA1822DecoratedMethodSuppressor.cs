using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vogen.Suppressors;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class CA1822DecoratedMethodSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor _ruleForValidateMethod = new(
        id: "VOGS0001",
        suppressedDiagnosticId: "CA1822",
        justification: "Suppress CA1822 on non-static Validate methods in a value object.");

    private static readonly SuppressionDescriptor _ruleForNormalizeInputMethod = new(
        id: "VOGS0002",
        suppressedDiagnosticId: "CA1822",
        justification: "Suppress CA1822 on non-static NormalizeInput methods in a value object.");

    // ReSharper disable once UseCollectionExpression
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
        ImmutableArray.Create(_ruleForValidateMethod, _ruleForNormalizeInputMethod);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
            var attributeSymbol = context.Compilation.GetBestTypeByMetadataName("Vogen.ValueObjectAttribute");
            
            if (attributeSymbol is null)
            {
                return;
            }

            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                ProcessDiagnostic(context, diagnostic);
            }
    }

    private static void ProcessDiagnostic(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
        var node = diagnostic.TryFindNode(context.CancellationToken);
        
        if (node is null)
        {
            return;
        }

        var semanticModel = context.GetSemanticModel(node.SyntaxTree);
        
        var methodSymbol = semanticModel.GetDeclaredSymbol(node, context.CancellationToken) as IMethodSymbol;
        
        if (methodSymbol is null)
        {
            return;
        }

        INamedTypeSymbol? containingType = methodSymbol.ContainingType;

        if (!VoFilter.IsTarget(containingType))
        {
            return;
        }

        if (methodSymbol.Name is "Validate")
        {
            var suppression = Suppression.Create(_ruleForValidateMethod, diagnostic);
            context.ReportSuppression(suppression);
        }

        if (methodSymbol.Name is "NormalizeInput")
        {
            var suppression = Suppression.Create(_ruleForNormalizeInputMethod, diagnostic);
            context.ReportSuppression(suppression);
        }
    }
}