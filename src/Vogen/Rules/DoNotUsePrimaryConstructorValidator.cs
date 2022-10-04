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
public class DoNotUsePrimaryConstructorAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.NoNotUsePrimaryConstructor,
        "Primary constructors on Value Objects are prohibited - creation is done via the From method",
        "Type '{0}' cannot have a primary constructor",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object has a primary constructor. This can lead to invalid value objects in your domain. Use the From method instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get { return ImmutableArray.Create(_rule); }
    }

    public override void Initialize(AnalysisContext context)
    {

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // we can't wait until compilation because the generate will generate invalid code, e.g.
        // `error CS8862: A constructor declared in a record with parameter list must have 'this' constructor initializer.`
        context.RegisterSyntaxNodeAction(AnalyzeSymbol3, SyntaxKind.RecordDeclaration);
        // context.RegisterCompilationStartAction(compilationContext =>
        // {
        //     compilationContext.RegisterSyntaxNodeAction(AnalyzeSymbol3, SyntaxKind.RecordDeclaration);
        // });
    }

    private void AnalyzeSymbol3(SyntaxNodeAnalysisContext ctx)
    {
        RecordDeclarationSyntax? rds = ctx.Node.Parent as RecordDeclarationSyntax;

        if (rds == null) return;

        if (!VoFilter.HasValueObjectAttribute(rds.AttributeLists, ctx.SemanticModel)) return;
        
        ReportIfNeeded(ctx, rds);
    }

    private static void ReportIfNeeded(SyntaxNodeAnalysisContext ctx, RecordDeclarationSyntax literalExpressionSyntax)
    {
        var typeInfo = ctx.SemanticModel.GetTypeInfo(literalExpressionSyntax).Type;
        if (typeInfo is not INamedTypeSymbol symbol) return;
        //
        // ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        //
        // if (attributes.Length == 0) return;
        //
        // AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");
        //
        // if (attr is null) return;

        var diagnostic = DiagnosticItems.BuildDiagnostic(_rule, symbol.Name, literalExpressionSyntax.GetLocation());

        ctx.ReportDiagnostic(diagnostic);
    }
}