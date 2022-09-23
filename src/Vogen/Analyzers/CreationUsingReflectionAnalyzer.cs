using System.Collections.Immutable;
using System.Linq;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Diagnostics;

namespace Vogen.Analyzers;

/// <summary>
/// An analyzer that stops `CustomerId = default;`.
/// </summary>
[Generator]
public class CreationUsingReflectionAnalyzer : IIncrementalGenerator
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

    private static IncrementalValuesProvider<FoundItem?> GetTargets(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is InvocationExpressionSyntax,
                transform: static (ctx, _) => TryGetTarget(ctx))
            .Where(static m => m is not null);
    }

    private static FoundItem? TryGetTarget(GeneratorSyntaxContext ctx)
    {
        var syntax = (InvocationExpressionSyntax) ctx.Node;
        var methodSymbol = (ctx.SemanticModel.GetSymbolInfo(syntax).Symbol as IMethodSymbol);
        if (methodSymbol == null)
        {
            return null;
        }

        if (methodSymbol.ReceiverType?.FullNamespace() != "System") return null;
        if (methodSymbol.ReceiverType.Name != "Activator") return null;

        if (methodSymbol.Name != "CreateInstance")
        {
            return null;
        }

        if (methodSymbol.Parameters.Length == 0)
        {
            if (!methodSymbol.IsGenericMethod) return null;

            var returnType = methodSymbol.ReturnType as INamedTypeSymbol;

            if (returnType == null) return null;
            
            if (!VoFilter.IsTarget(returnType)) return null;
            
            return new FoundItem(syntax.GetLocation(), returnType);
        }

        if (methodSymbol.Parameters.Length == 1)
        {
            var childNodes = syntax.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList();

            if (childNodes.Count != 1) return null;

            TypeInfo xxy = ctx.SemanticModel.GetTypeInfo(childNodes[0].Type);

            var syntaxNode = xxy.Type as INamedTypeSymbol;
            
            if (!VoFilter.IsTarget(syntaxNode)) return null;

            return new FoundItem(syntax.GetLocation(), syntaxNode!);
        }
        
        return null;
    }

    static void Execute(ImmutableArray<FoundItem?> typeDeclarations, SourceProductionContext context)
    {
        foreach (FoundItem? eachFoundItem in typeDeclarations)
        {
            if (eachFoundItem is not null)
            {
                context.ReportDiagnostic(DiagnosticItems.UsingActivatorProhibited(eachFoundItem.Value.Location, eachFoundItem.Value.VoClass.Name));
            }
        }
    }
}