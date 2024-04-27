using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal static class VoFilter
{
    public static IEnumerable<AttributeData> TryGetValueObjectAttributes(INamedTypeSymbol voSymbolInformation)
    {
        var attrs = voSymbolInformation.GetAttributes();

        return attrs.Where(
                a => a.AttributeClass?.FullName() == "Vogen.ValueObjectAttribute"
                     || a.AttributeClass?.BaseType?.FullName() == "Vogen.ValueObjectAttribute"
                     || a.AttributeClass?.BaseType?.BaseType?.FullName() == "Vogen.ValueObjectAttribute");
    }

    public static VoTarget? TryGetTarget(GeneratorSyntaxContext context)
    {
        var voSyntaxInformation = (TypeDeclarationSyntax) context.Node;

        var semanticModel = context.SemanticModel;

        var declaredSymbol = semanticModel.GetDeclaredSymbol(context.Node)!;
        
        var voSymbolInformation = (INamedTypeSymbol) declaredSymbol;

        var attributeData = TryGetValueObjectAttributes(voSymbolInformation).ToImmutableArray();

        if (attributeData.Length > 0)
        {
            return new VoTarget(
                semanticModel: semanticModel,
                typeToAugment: voSyntaxInformation,
                nestingInfo: declaredSymbol.ContainingType is not null
                    ? NestingInfo.AsNestedIn(declaredSymbol.ContainingType)
                    : NestingInfo.NotNested,
                symbolForType: voSymbolInformation,
                dataForAttributes: attributeData);
        }

        return null;
    }

    public static bool IsTarget(SyntaxNode syntaxNode) => 
        syntaxNode is TypeDeclarationSyntax { AttributeLists.Count: > 0 };

    public static bool IsTarget(INamedTypeSymbol? voClass) => 
        voClass is not null && TryGetValueObjectAttributes(voClass).Any();    
}
