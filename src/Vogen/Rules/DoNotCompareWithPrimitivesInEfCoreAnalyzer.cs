using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotCompareWithPrimitivesInEfCoreAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotCompareWithPrimitivesInEfCore,
        "Comparing primitives with value objects in EFCore expressions can cause casting issues",
        "Value object '{0}' is being compared to an int. Compare it with the value object instead.",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "The value object is being compared with a primitive in an EFCore expression. This can lead to EFCore trying and failing to cast between the two.");


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        // Use Analyze | ReportDiagnostics so user code inside Blazor/Razor-generated C# files is checked.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(OnCompilationStart);
    }
    
    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var hasEfCoreRef = context.Compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbSet`1") is not null;

        if (!hasEfCoreRef)
        {
            return;
        }
        
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (VoFilter.IsInCodeThatShouldNotBeAnalyzed(context.Node)) return;

        var invocationExpr = (InvocationExpressionSyntax) context.Node;

        if (context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (!EfCoreAnalyzerHelper.IsLinqToEntities(methodSymbol, context.SemanticModel.Compilation))
        {
            return;
        }

        foreach (ArgumentSyntax eachArgument in
                 invocationExpr.ArgumentList.Arguments.Where(e => e.Expression is LambdaExpressionSyntax))
        {
            foreach (BinaryExpressionSyntax eachBinaryExpression in eachArgument.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                ITypeSymbol? left = context.SemanticModel.GetTypeInfo(eachBinaryExpression.Left).Type;
                ITypeSymbol? right = context.SemanticModel.GetTypeInfo(eachBinaryExpression.Right).Type;

                if (left is null || right is null)
                {
                    continue;
                }

                // Check if left is ValueObject and right is integer
                if (EfCoreAnalyzerHelper.IsValueObject(left) && right.SpecialType == SpecialType.System_Int32)
                {
                    context.ReportDiagnostic(
                        DiagnosticsCatalogue.BuildDiagnostic(_rule, left.Name, eachBinaryExpression.GetLocation()));
                }
            }
        }
    }

    private static void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
    {
        if (VoFilter.IsInCodeThatShouldNotBeAnalyzed(context.Node)) return;

        var queryExpr = (QueryExpressionSyntax) context.Node;

        if (!EfCoreAnalyzerHelper.IsEfQuery(queryExpr, context.SemanticModel))
        {
            return;
        }

        var whereClauses = queryExpr.Body.DescendantNodes().OfType<WhereClauseSyntax>();

        foreach (var eachArgument in whereClauses)
        {
            foreach (BinaryExpressionSyntax eachBinaryExpression in eachArgument.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                ITypeSymbol? left = context.SemanticModel.GetTypeInfo(eachBinaryExpression.Left).Type;
                ITypeSymbol? right = context.SemanticModel.GetTypeInfo(eachBinaryExpression.Right).Type;

                if (left is null || right is null)
                {
                    continue;
                }

                // Check if left is ValueObject and right is integer
                if (EfCoreAnalyzerHelper.IsValueObject(left) && right.SpecialType == SpecialType.System_Int32)
                {
                    context.ReportDiagnostic(
                        DiagnosticsCatalogue.BuildDiagnostic(_rule, left.Name, eachBinaryExpression.GetLocation()));
                }
            }
        }
    }

}