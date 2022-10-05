using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

/// <summary>
/// An analyzer that stops `CustomerId = default;` and `CustomerId = default(CustomerId)`.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseDefaultAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseDefault,
        "Using default of Value Objects is prohibited",
        "Type '{0}' cannot be constructed with default as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a default expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterSyntaxNodeAction(AnalyzeLiteral, SyntaxKind.DefaultLiteralExpression);
            compilationContext.RegisterSyntaxNodeAction(Analyze, SyntaxKind.DefaultExpression);
        });
    }

    private static void Analyze(SyntaxNodeAnalysisContext ctx)
    {
        var literalExpressionSyntax = (DefaultExpressionSyntax) ctx.Node;

        if (literalExpressionSyntax.Kind() != SyntaxKind.DefaultExpression)
        {
            return;
        }

        ReportIfNeeded(ctx, literalExpressionSyntax);
    }

    private static void AnalyzeLiteral(SyntaxNodeAnalysisContext ctx)
    {
        var literalExpressionSyntax = (LiteralExpressionSyntax) ctx.Node;

        if (literalExpressionSyntax.Kind() != SyntaxKind.DefaultLiteralExpression)
        {
            return;
        }
        
        ReportIfNeeded(ctx, literalExpressionSyntax);
    }

    private static void ReportIfNeeded(SyntaxNodeAnalysisContext ctx, ExpressionSyntax literalExpressionSyntax)
    {
        var typeInfo = ctx.SemanticModel.GetTypeInfo(literalExpressionSyntax).Type;
        if (typeInfo is not INamedTypeSymbol symbol) return;

        if (!VoFilter.IsTarget(symbol)) return;

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, symbol.Name, literalExpressionSyntax.GetLocation());

        ctx.ReportDiagnostic(diagnostic);
    }
}