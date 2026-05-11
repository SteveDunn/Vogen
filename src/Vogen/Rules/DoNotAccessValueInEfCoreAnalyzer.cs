using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotAccessValueInEfCoreAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new(
        RuleIdentifiers.DoNotAccessValueInEfCore,
        "Directly accessing the value in EFCore expressions cannot be translated",
        "The value of object '{0}' is being accessed directly. Cast it to its primitive instead.",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "EFCore cannot translate the access to the underlying value of an value object. You need to cast the object to its primitive.");


    public override void Initialize(AnalysisContext context)
    {
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
            foreach (MemberAccessExpressionSyntax eachMemberAccess in eachArgument.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
            {
                if (eachMemberAccess.Name.Identifier.Text != "Value")
                {
                    continue;
                }

                ITypeSymbol? expressionType = context.SemanticModel.GetTypeInfo(eachMemberAccess.Expression).Type;

                if (expressionType is null)
                {
                    continue;
                }

                if (EfCoreAnalyzerHelper.IsValueObject(expressionType))
                {
                    context.ReportDiagnostic(
                        DiagnosticsCatalogue.BuildDiagnostic(_rule, expressionType.Name, eachMemberAccess.GetLocation()));
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

        foreach (MemberAccessExpressionSyntax eachMemberAccess in queryExpr.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
        {
            if (eachMemberAccess.Name.Identifier.Text != "Value")
            {
                continue;
            }

            ITypeSymbol? expressionType = context.SemanticModel.GetTypeInfo(eachMemberAccess.Expression).Type;

            if (expressionType is null)
            {
                continue;
            }

            if (EfCoreAnalyzerHelper.IsValueObject(expressionType))
            {
                context.ReportDiagnostic(
                    DiagnosticsCatalogue.BuildDiagnostic(_rule, expressionType.Name, eachMemberAccess.GetLocation()));
            }
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [_rule];
}