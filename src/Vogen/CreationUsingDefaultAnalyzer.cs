using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

/// <summary>
/// An analyzer that stops `CustomerId = default;`.
/// </summary>
[Generator]
public class CreationUsingDefaultAnalyzer : IIncrementalGenerator
{
    public record struct FoundItem(Location Location, INamedTypeSymbol VoClass);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<FoundItem?> targets = GetTargets(context);

        IncrementalValueProvider<(Compilation, ImmutableArray<FoundItem?>)> compilationAndTypes
            = context.CompilationProvider.Combine(targets.Collect());
            
        context.RegisterSourceOutput(compilationAndTypes,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static IncrementalValuesProvider<FoundItem?> GetTargets(IncrementalGeneratorInitializationContext context) =>
        context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is DefaultExpressionSyntax,
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        var defaultExpressionSyntax = (DefaultExpressionSyntax) ctx.Node;

        TypeSyntax typeSyntax = defaultExpressionSyntax.Type;

        INamedTypeSymbol? voClass = VoFilter.TryGetValueObjectClass(ctx, typeSyntax);
        
        return voClass == null ? null : new FoundItem
        {
            VoClass = voClass,
            Location = typeSyntax.GetLocation()
        };
    }

    static void Execute(
        Compilation _, 
        ImmutableArray<FoundItem?> typeDeclarations,
        SourceProductionContext context)
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