using System.Collections.Immutable;
using System.Linq;
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
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotUseDefault,
        "Using default of Value Objects is prohibited",
        "Type '{0}' cannot be constructed with default as it is prohibited",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is created with a default expression. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get { return ImmutableArray.Create(_rule); }
    }

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterSyntaxNodeAction(AnalyzeExpressionSyntax, SyntaxKind.DefaultLiteralExpression);
            compilationContext.RegisterSyntaxNodeAction(AnalyzeExpressionSyntax2, SyntaxKind.DefaultExpression);
        });
    }

    private void AnalyzeExpressionSyntax2(SyntaxNodeAnalysisContext ctx)
    {
        var literalExpressionSyntax = (DefaultExpressionSyntax) ctx.Node;

        if (literalExpressionSyntax.Kind() != SyntaxKind.DefaultExpression)
        {
            return;
        }

        ReportIfNeeded(ctx, literalExpressionSyntax);
    }

    private void AnalyzeExpressionSyntax(SyntaxNodeAnalysisContext ctx)
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

        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

        if (attributes.Length == 0) return;

        AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (attr is null) return;

        var diagnostic = DiagnosticItems.BuildDiagnostic(_rule, symbol.Name, literalExpressionSyntax.GetLocation());

        ctx.ReportDiagnostic(diagnostic);
    }
}