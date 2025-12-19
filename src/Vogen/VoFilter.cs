using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal static class VoFilter
{
    /// <summary>
    /// Tries to get any `ValueObject` attributes specified on the provided symbol.
    /// It might return more than one, because the user might have typed more than one.
    /// Even though having more than one is not valid, it's still possible for it to exist,
    /// so we return what is found and let the caller decide what to do.
    /// </summary>
    /// <param name="voSymbolInformation"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> TryGetValueObjectAttributes(INamedTypeSymbol voSymbolInformation)
    {
        var attrs = voSymbolInformation.GetAttributes();

        return attrs.Where(
            a => a.AttributeClass?.EscapedFullName() == "Vogen.ValueObjectAttribute"
            || a.AttributeClass?.BaseType?.EscapedFullName() == "Vogen.ValueObjectAttribute"
            || a.AttributeClass?.BaseType?.BaseType?.EscapedFullName() == "Vogen.ValueObjectAttribute");
    }

    /// <summary>
    /// Given a type declaration (via the context), it gets the semantic model,
    /// then gets the type declaration, then tries to get any value object
    /// attributes.
    /// </summary>
    /// <param name="context">The context containing the node.</param>
    /// <returns>The target, otherwise null.</returns>
    public static VoTarget? TryGetTarget(GeneratorSyntaxContext context)
    {
        var voSyntaxInformation = (TypeDeclarationSyntax) context.Node;

        var semanticModel = context.SemanticModel;

        ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(context.Node)!;

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