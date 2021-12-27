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
    public record struct FoundItem(Location Location, INamedTypeSymbol VoClass);

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

        var typeSyntax = GetTypeFromVariableOrParameter(literalExpressionSyntax);
        
        if (typeSyntax is null)
        {
            return null;
        }
        
        INamedTypeSymbol? voClass = VoFilter.TryGetValueObjectClass(ctx, typeSyntax);

        return voClass is null ? null : new FoundItem(typeSyntax.GetLocation(), voClass);
    }
    
    // A default literal expression can be for a variable (CustomerId id = default), or
    // a parameter (void DoSomething(CustomerId id = default)).
    // We need to try to find the 'Type' from either one of those type.
    private static TypeSyntax? GetTypeFromVariableOrParameter(LiteralExpressionSyntax literalExpressionSyntax)
    {
        
        var ancestor = literalExpressionSyntax.Ancestors(false)
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