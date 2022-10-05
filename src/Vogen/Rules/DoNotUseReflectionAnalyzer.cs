using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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

        context.RegisterCompilationStartAction(compilationContext =>
        {
            compilationContext.RegisterSyntaxNodeAction(AnalyzeExpressionSyntax, SyntaxKind.InvocationExpression);
        });
    }

    private static void AnalyzeExpressionSyntax(SyntaxNodeAnalysisContext ctx)
    {
        var invocationSyntax = (InvocationExpressionSyntax) ctx.Node;

        if (invocationSyntax.Kind() != SyntaxKind.InvocationExpression)
        {
            return;
        }
        
        var methodSymbol = (ctx.SemanticModel.GetSymbolInfo(invocationSyntax).Symbol as IMethodSymbol);
        if (methodSymbol == null) return;
        if (methodSymbol.ReceiverType?.FullNamespace() != "System") return;
        if (methodSymbol.ReceiverType.Name != "Activator") return;
        if (methodSymbol.Name != "CreateInstance") return;

        if (methodSymbol.Parameters.Length == 0)
        {
            if (!methodSymbol.IsGenericMethod) return;

            var typeSymbol = ctx.SemanticModel.GetTypeInfo(invocationSyntax).Type;
            ReportIfNeeded(typeSymbol, ctx, invocationSyntax.GetLocation());
        }

        if (methodSymbol.Parameters.Length == 1)
        {
            var childNodes = invocationSyntax.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList();

            if (childNodes.Count != 1) return;

            var typeSymbol = ctx.SemanticModel.GetTypeInfo(childNodes[0].Type).Type;
            ReportIfNeeded(typeSymbol, ctx, invocationSyntax.GetLocation());
        }
    }

    private static void ReportIfNeeded(ITypeSymbol? typeInfo, SyntaxNodeAnalysisContext ctx, Location location)
    {
        if (typeInfo is not INamedTypeSymbol symbol) return;
        
        if (!VoFilter.IsTarget(symbol)) return;

        // ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
        //
        // if (attributes.Length == 0) return;
        //
        // AttributeData? attr = attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");
        //
        // if (attr is null) return;

        var diagnostic = DiagnosticsCatalogue.BuildDiagnostic(_rule, symbol.Name, location);

        ctx.ReportDiagnostic(diagnostic);
    }
}