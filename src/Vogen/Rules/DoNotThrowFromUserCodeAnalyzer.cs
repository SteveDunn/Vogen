using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotThrowFromUserCodeAnalyzer : DiagnosticAnalyzer
{
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it won't pick this up when implied
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotThrowFromUserCode,
        "Value objects should not throw exceptions",
        "Type '{0}' throws an exception which can cause surprising side effects, for instance, in implicit conversions",
        RuleCategories.Usage,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Value objects can contain user code; methods such as NormalizeInput and Validate. These should not throw exceptions, because that can cause surprising side effects such as when doing implicit conversions (which should not throw). The only place to throw, and which is handle by the generated code, is an exception related to validation.");

    // ReSharper disable once UseCollectionExpression
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext ctx)
    {
        var throwStatement =  ctx.Node;
        
        var containingType = throwStatement.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().FirstOrDefault();
        if (containingType is null)
        {
            return;
        }

        var symbol = ctx.SemanticModel.GetDeclaredSymbol(containingType);

        if (!VoFilter.IsTarget(symbol))
        {
            return;
        }
        
        var containingMethod = throwStatement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();

        if (containingMethod?.Identifier.Text is not ("Validate" or "NormalizeInput"))
        {
            return;
        }

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, containingType.Identifier.Text, throwStatement.GetLocation());
        ctx.ReportDiagnostic(diagnostic);
    }
}