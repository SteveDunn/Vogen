using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

/// <summary>
/// An analyzer that stops `CustomerId = default(CustomerId);` and `void DoSomething(CustomerId id = default)`.
/// See also <see cref="CreationUsingDefaultAnalyzer"/>.
/// </summary>
[Generator]
public class CreationUsingDefaultLiteralAnalyzer : IIncrementalGenerator
{
    private record struct FoundItem(Location Location, INamedTypeSymbol VoClass);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<FoundItem?> targets = GetTargets(context);

        IncrementalValueProvider<(Compilation, ImmutableArray<FoundItem?>)> compilationAndTypes
            = context.CompilationProvider.Combine(targets.Collect());

        context.RegisterSourceOutput(compilationAndTypes,
            static (spc, source) => Execute(source.Item2, spc));
    }

    private static IncrementalValuesProvider<FoundItem?> GetTargets(IncrementalGeneratorInitializationContext context) =>
        context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is LiteralExpressionSyntax,
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        var literalExpressionSyntax = (LiteralExpressionSyntax) ctx.Node;

        if (literalExpressionSyntax.Kind() != SyntaxKind.DefaultLiteralExpression)
        {
            return null;
        }

        var typeSyntax = TryGetTypeFromVariableOrParameter(ctx, literalExpressionSyntax);

        if (typeSyntax is not null)
        {
            var classFromSyntax = VoFilter.TryGetValueObjectClass(ctx, typeSyntax);

            if (classFromSyntax is not null)
            {
                return new FoundItem(typeSyntax.GetLocation(), classFromSyntax);
            }
        }

        INamedTypeSymbol? classFromModel = TryGetTypeFromModel(ctx, literalExpressionSyntax);

        return classFromModel is null ? null : new FoundItem(literalExpressionSyntax.GetLocation(), classFromModel);
    }

    private static INamedTypeSymbol? TryGetTypeFromModel(GeneratorSyntaxContext ctx, LiteralExpressionSyntax literalExpressionSyntax)
    {
        // for lambdas, we need the semantic model...
        var voClass = TryGetFromLambda(ctx, literalExpressionSyntax);
        return voClass;

    }

    // A default literal expression can be for a variable (CustomerId id = default), or
    // a parameter (void DoSomething(CustomerId id = default)).
    // We need to try to find the 'Type' from either one of those type.
    private static TypeSyntax? TryGetTypeFromVariableOrParameter(
        GeneratorSyntaxContext ctx,
        LiteralExpressionSyntax literalExpressionSyntax)
    {
        // first, see if it's an array
        var ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.ArrayCreationExpression));

        if (ancestor is ArrayCreationExpressionSyntax arraySyntax)
        {
            return arraySyntax.Type.ElementType;
        }

        ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.LocalFunctionStatement));

        if (ancestor is LocalFunctionStatementSyntax syntax)
        {
            return syntax.ReturnType;
        }

        ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.MethodDeclaration));

        if (ancestor is MethodDeclarationSyntax methodSyntax)
        {
            return methodSyntax.ReturnType;
        }

        ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.VariableDeclaration));

        if (ancestor is VariableDeclarationSyntax variableDeclarationSyntax)
        {
            return variableDeclarationSyntax.Type;
        }

        ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.Parameter));

        if (ancestor is ParameterSyntax parameterSyntax)
        {
            return parameterSyntax.Type;
        }

        return null;
    }

    private static INamedTypeSymbol? TryGetFromLambda(
        GeneratorSyntaxContext ctx,
        SyntaxNode literalExpressionSyntax)
    {
        var ancestor = literalExpressionSyntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.ParenthesizedLambdaExpression));

        if (ancestor is not ParenthesizedLambdaExpressionSyntax lambdaExpressionSyntax)
        {
            return null;
        }

        var info = ctx.SemanticModel.GetSymbolInfo(lambdaExpressionSyntax);

        if (info.Symbol is not IMethodSymbol ms)
        {
            return null;
        }

        var returnTypeSymbol = ms.ReturnType as INamedTypeSymbol;

        if (VoFilter.TryGetValueObjectClass(ctx, returnTypeSymbol))
        {
            return returnTypeSymbol;
        }

        return null;
    }

    static void Execute(ImmutableArray<FoundItem?> typeDeclarations, SourceProductionContext context)
    {
        foreach (FoundItem? eachFoundItem in typeDeclarations)
        {
            if (eachFoundItem is not null)
            {
                context.ReportDiagnostic(
                    DiagnosticCollection.UsingDefaultProhibited(eachFoundItem.Value.Location, eachFoundItem.Value.VoClass.Name));
            }
        }
    }
}