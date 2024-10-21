using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vogen.Diagnostics;

namespace Vogen.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotCompareWithPrimitivesInEfCoreAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> _knownNames = new[] { "Where", "Single", "SkipWhile", "TakeWhile", "Select" }.ToImmutableHashSet();

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident - current bug in Roslyn analyzer means it
    // won't find this and will report:
    // "error RS2002: Rule 'XYZ123' is part of the next unshipped analyzer release, but is not a supported diagnostic for any analyzer"
    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        RuleIdentifiers.DoNotCompareWithPrimitivesInEfCore,
        "Comparing primitives with value objects in EFCore expressions can cause casting issues",
        "Value object '{0}' is being compared to an int. Compare it with the value object instead.",
        RuleCategories.Usage,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description:
        "The value object is being compared with a primitive in an EFCore expression. This can lead to EFCore trying and failing to cast between the two.");


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.EqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var equalsExpression = (BinaryExpressionSyntax) context.Node;

        // Check if the expression is within a Where clause
        if (!(equalsExpression.Parent is LambdaExpressionSyntax lambdaParent)) return;
        if (!(lambdaParent.Parent is ArgumentSyntax argParent)) return;
        if (!(argParent.Parent is ArgumentListSyntax argListParent)) return;
        if (!(argListParent.Parent is InvocationExpressionSyntax invocationParent)) return;

        // Ensure it is the correct method
        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationParent).Symbol as IMethodSymbol;
        if (methodSymbol is null) return;

        // Get the left and right hand sides
        var left = context.SemanticModel.GetTypeInfo(equalsExpression.Left).Type!;
        var right = context.SemanticModel.GetTypeInfo(equalsExpression.Right).Type!;

        if (!IsEfCore(methodSymbol)) return;

        // Check if left is ValueObject and right is integer
        if (IsValueObject(left) && right.SpecialType == SpecialType.System_Int32)
        {
            context.ReportDiagnostic(DiagnosticsCatalogue.BuildDiagnostic(_rule, left.Name, equalsExpression.GetLocation()));

            //var diagnostic = Diagnostic.Create(_rule, equalsExpression.GetLocation());
            //context.ReportDiagnostic(diagnostic);
        }


        bool IsEfCore(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.Name != "Where") return false;

            if (methodSymbol.ReceiverType is not INamedTypeSymbol namedTypeSymbol)
            {
                return false;
            }

            // Check if the type is from the EF Core namespace
            INamespaceSymbol? ns = namedTypeSymbol.ContainingNamespace;
            if (ns is null) return false;

            foreach (var interfaceType in namedTypeSymbol.AllInterfaces)
            {
                if (interfaceType.ConstructedFrom.ToString() == "System.Linq.IQueryable")
                {
                    // Find the Provider property
                    foreach (var member in interfaceType.GetMembers())
                    {
                        if (member is IPropertySymbol propertySymbol && propertySymbol.Name == "Provider")
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }

    private static bool IsValueObject(ITypeSymbol type) => 
        type is INamedTypeSymbol symbol && VoFilter.IsTarget(symbol);

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax) context.Node;
        if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr) return;

        if (!_knownNames.Contains(memberAccessExpr.Name.Identifier.Text))
        {
            return;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpr.Expression);
        if (symbolInfo.Symbol is not IPropertySymbol ps) return;

        var dbSetType = context.SemanticModel.Compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbSet`1");
        if (dbSetType is null) return;

        if (!InheritsFrom(ps.Type, dbSetType)) return;

        foreach (var lambdaExpression in invocationExpr.ArgumentList.Arguments.Where(e => e.Expression is LambdaExpressionSyntax))
        {
            foreach (var binaryExpression in lambdaExpression.DescendantNodes().OfType<BinaryExpressionSyntax>())
            {
                ITypeSymbol? left = context.SemanticModel.GetTypeInfo(binaryExpression.Left).Type;
                ITypeSymbol? right = context.SemanticModel.GetTypeInfo(binaryExpression.Right).Type;

                if (left is null || right is null) continue;

                // Check if left is ValueObject and right is integer
                if (IsValueObject(left) && right.SpecialType == SpecialType.System_Int32)
                {
                    context.ReportDiagnostic(DiagnosticsCatalogue.BuildDiagnostic(_rule, left.Name, binaryExpression.GetLocation()));
                }
            }
        }


        // var diagnostic = Diagnostic.Create(_rule, memberAccessExpr.GetLocation(), ps.Name);
        // context.ReportDiagnostic(diagnostic);
    }

    private static bool InheritsFrom(ITypeSymbol type, INamedTypeSymbol baseType)
    {
        while (type != null)
        {
            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, baseType))
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }
}