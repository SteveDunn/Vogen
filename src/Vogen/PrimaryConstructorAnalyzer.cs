using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

/// <summary>
/// An analyzer that stops record VOs having a primary constructor.
/// </summary>
[Generator]
public class PrimaryConstructorAnalyzer : IIncrementalGenerator
{
    private record struct FoundItem(Location Location, string Name);

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
                predicate: static (s, _) => s is ParameterListSyntax,
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        RecordDeclarationSyntax? rds = ctx.Node.Parent as RecordDeclarationSyntax;

        if(rds == null)
        {
            return null;
        }

        if (!VoFilter.HasValueObjectAttribute(rds.AttributeLists, ctx))
        {
            return null;
        }

        return new FoundItem
        {
            Name = rds.Identifier.Value?.ToString()!,
            Location = rds.GetLocation()
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
                    DiagnosticItems.PrimaryConstructorProhibited(eachFoundItem.Value.Location, eachFoundItem.Value.Name));
            }
        }
    }
}