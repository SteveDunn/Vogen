using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen;

/// <summary>
/// An analyzer that stops target type new (implicit new): `CustomerId c = new()`.
/// See also <see cref="CreationUsingNewAnalyzer"/>, which handles the normal new operation.
/// </summary>
[Generator]
public class CreationUsingImplicitNewAnalyzer : IIncrementalGenerator
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
                predicate: static (s, _) => s is ImplicitObjectCreationExpressionSyntax,
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        var syntax = (ImplicitObjectCreationExpressionSyntax) ctx.Node;
        
        var ancestor = syntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.VariableDeclaration));

        if (ancestor is VariableDeclarationSyntax variableDeclarationSyntax)
        {
            TypeSyntax t = variableDeclarationSyntax.Type;
            INamedTypeSymbol? voClass = VoFilter.TryGetValueObjectClass(ctx, t);

            return voClass == null ? null : new FoundItem
            {
                VoClass = voClass,
                Location = t.GetLocation()
            };
        }

        ancestor = syntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.MethodDeclaration));

        if (ancestor is MethodDeclarationSyntax methodSyntax)
        {
            TypeSyntax t = methodSyntax.ReturnType;
            INamedTypeSymbol? voClass = VoFilter.TryGetValueObjectClass(ctx, t);

            return voClass == null ? null : new FoundItem
            {
                VoClass = voClass,
                Location = t.GetLocation()
            };
        }

        ancestor = syntax.Ancestors(false)
            .FirstOrDefault(a => a.IsKind(SyntaxKind.LocalFunctionStatement));

        if (ancestor is LocalFunctionStatementSyntax localFunctionStatementSyntax)
        {
            TypeSyntax t = localFunctionStatementSyntax.ReturnType;
            INamedTypeSymbol? voClass = VoFilter.TryGetValueObjectClass(ctx, t);

            return voClass == null ? null : new FoundItem
            {
                VoClass = voClass,
                Location = t.GetLocation()
            };
        }

        return null;
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
                    DiagnosticCollection.UsingNewProhibited(eachFoundItem.Value.Location, eachFoundItem.Value.VoClass.Name));
            }
        }
    }
}