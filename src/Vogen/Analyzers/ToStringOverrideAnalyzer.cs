using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen.Analyzers;

/// <summary>
/// An analyzer that catches ToString overrides on a record that aren't sealed.
/// For more information on why, see: https://github.com/SteveDunn/Vogen/wiki/Records#tostring
/// </summary>
[Generator]
public class ToStringOverrideAnalyzer : IIncrementalGenerator
{
    private record struct FoundItem(Location Location, string Name);

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
                predicate: static (s, _) => s is MethodDeclarationSyntax mds && mds.Identifier.ToString() == "ToString",
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        var syntax = (MethodDeclarationSyntax) ctx.Node;

        RecordDeclarationSyntax? rds = ctx.Node.Parent as RecordDeclarationSyntax;

        if (rds == null)
        {
            return null;
        }

        if (!VoFilter.HasValueObjectAttribute(rds.AttributeLists, ctx))
        {
            return null;
        }
        
        if (!(syntax.Modifiers.Any(SyntaxKind.PublicKeyword) && syntax.Modifiers.Any(SyntaxKind.OverrideKeyword)))
        {
            return null;
        }

        if (syntax.Modifiers.Any(SyntaxKind.SealedKeyword))
        {
            return null;
        }
        
        return new FoundItem
        {
            Name = rds.Identifier.Value?.ToString()!,
            Location = syntax.GetLocation()
        };
    }

    static void Execute(ImmutableArray<FoundItem?> typeDeclarations, SourceProductionContext context)
    {
        foreach (FoundItem? eachFoundItem in typeDeclarations)
        {
            if (eachFoundItem is not null)
            {
                context.ReportDiagnostic(
                    DiagnosticItems.RecordToStringOverloadShouldBeSealed(eachFoundItem.Value.Location, eachFoundItem.Value.Name));
            }
        }
    }
}