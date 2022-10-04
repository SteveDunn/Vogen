using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

    public static bool HasValueObjectAttribute(SyntaxList<AttributeListSyntax> attributeList, SemanticModel semanticModel)
    {
        foreach (AttributeListSyntax attributeListSyntax in attributeList)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                IMethodSymbol? attributeSymbol = semanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;

                if (attributeSymbol == null)
                {
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Vogen.ValueObjectAttribute")
                {
                    return true;
                }
            }
        }

        return false;
    }

    // public static bool HasValueObjectAttribute(SyntaxList<AttributeListSyntax> attributeList, SemanticModel contextSemanticModel)
    // {
    //     foreach (AttributeListSyntax attributeListSyntax in attributeList)
    //     {
    //         foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
    //         {
    //             IMethodSymbol? attributeSymbol = contextSemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
    //
    //             if (attributeSymbol == null)
    //             {
    //                 continue;
    //             }
    //
    //             INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
    //             string fullName = attributeContainingTypeSymbol.ToDisplayString();
    //
    //             if (fullName == "Vogen.ValueObjectAttribute")
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //
    //     return false;
    // }

    // This is stage 2 in the pipeline - we filter down to just 1 target
    public static VoTarget? TryGetTarget(GeneratorSyntaxContext context)
    {
        var voSyntaxInformation = (TypeDeclarationSyntax) context.Node;

        var voSymbolInformation = (INamedTypeSymbol) context.SemanticModel.GetDeclaredSymbol(context.Node)!;
        
        if(HasValueObjectAttribute(voSyntaxInformation.AttributeLists, context.SemanticModel))
        {
            return new VoTarget(
                context.SemanticModel,
                voSyntaxInformation, 
                context.SemanticModel.GetDeclaredSymbol(context.Node)!.ContainingType,
                voSymbolInformation);
        }

        return null;
    }

    public static bool IsTarget(INamedTypeSymbol? voClass)
    {
        if (voClass == null)
        {
            return false;
        }

        ImmutableArray<AttributeData> attributes = voClass.GetAttributes();

        if (attributes.Length == 0)
        {
            return false;
        }

        AttributeData? voAttribute =
            attributes.SingleOrDefault(a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        return voAttribute is not null;
    }

    public static INamedTypeSymbol? TryGetValueObjectClass(GeneratorSyntaxContext context, SyntaxNode syntaxNode)
    {
        SymbolInfo typeSymbolInfo = context.SemanticModel.GetSymbolInfo(syntaxNode);

        var symbol = typeSymbolInfo.Symbol as INamedTypeSymbol;
        
        return IsTarget(symbol) ? symbol : null;
    }
}