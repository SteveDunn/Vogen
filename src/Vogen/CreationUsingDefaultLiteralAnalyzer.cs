using System.Collections.Immutable;
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

        var typeInfo = ctx.SemanticModel.GetTypeInfo(literalExpressionSyntax).Type;
        if (typeInfo is INamedTypeSymbol typeSymbol)
        {
            return VoFilter.IsTarget(typeSymbol)
                ? new FoundItem(literalExpressionSyntax.GetLocation(), typeSymbol)
                : null;
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
                    DiagnosticItems.UsingDefaultProhibited(eachFoundItem.Value.Location, eachFoundItem.Value.VoClass.Name));
            }
        }
    }
}