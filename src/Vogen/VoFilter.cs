using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal static class VoFilter
{
    /// <summary>
    /// This is stage 1 in the pipeline - the 'quick filter'.  We find out is it a type declaration and does it have any attributes? - don't allocate anything
    /// here as this is called a **lot** (every time the editor is changed, i.e. key-presses).
    /// </summary>
    /// <param name="syntaxNode"></param>
    /// <returns></returns>
    public static bool IsTarget(SyntaxNode syntaxNode) =>
        syntaxNode is TypeDeclarationSyntax t && t.AttributeLists.Count > 0;

    // This is stage 2 in the pipeline - we filter down to just 1 target
    public static VoTarget? TryGetTarget(GeneratorSyntaxContext context)
    {
        var tds = (TypeDeclarationSyntax) context.Node;

        var voClass = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(context.Node)!;

        foreach (AttributeListSyntax attributeListSyntax in tds.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                IMethodSymbol? attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;

                if (attributeSymbol == null)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Vogen.ValueObjectAttribute")
                {
                    return new VoTarget(
                        tds, 
                        context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType,
                        voClass);
                }
            }
        }

        return null;
    }

    public static INamedTypeSymbol? TryGetValueObjectClass(GeneratorSyntaxContext context, SyntaxNode syntaxNode)
    {
        SymbolInfo typeSymbolInfo = context.SemanticModel.GetSymbolInfo(syntaxNode);

        var symbol = typeSymbolInfo.Symbol as INamedTypeSymbol;
        
        return IsTarget(symbol) ? symbol : null;
    }

    public static bool IsTarget(INamedTypeSymbol? voClass)
    {
        if (voClass == null)
        {
            return false;
        }

        var attributes = voClass.GetAttributes();

        if (attributes.Length == 0)
        {
            return false;
        }

        AttributeData? voAttribute =
            attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        return voAttribute is not null;
    }
}