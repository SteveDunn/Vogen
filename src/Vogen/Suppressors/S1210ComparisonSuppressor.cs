using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vogen.Suppressors;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class S1210ComparisonSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor _suppressIComparableWarning = new(
        id: "VOGS0004",
        suppressedDiagnosticId: "S1210",
        justification: "Suppress S1210 on value objects.");

    // ReSharper disable once UseCollectionExpression
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
        ImmutableArray.Create(_suppressIComparableWarning);

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

        var valueObjectSymbol = semanticModel.GetDeclaredSymbol(node, context.CancellationToken) as INamedTypeSymbol;

        if (!VoFilter.IsTarget(valueObjectSymbol))
        {
            return;
        }

        var suppression = Suppression.Create(_suppressIComparableWarning, diagnostic);
        context.ReportSuppression(suppression);
    }
}