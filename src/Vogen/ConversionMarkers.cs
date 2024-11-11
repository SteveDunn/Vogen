using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal static class ConversionMarkers
{
    /// <summary>
    /// Tries to get any `ValueObject` attributes specified on the provided symbol.
    /// It might return more than one, because the user might have typed more than one.
    /// Even though having more than one is not valid, it's still possible for it to exist,
    /// so we return what is found and let the caller decide what to do.
    /// </summary>
    /// <param name="voSymbolInformation"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> TryGetMarkerAttributes(INamedTypeSymbol voSymbolInformation)
    {
        var attrs = voSymbolInformation.GetAttributes();

        return attrs.Where(
            a => a.AttributeClass?.MetadataName == "EfCoreConverterAttribute`1"
                 || a.AttributeClass?.MetadataName == "MessagePackAttribute`1");
    }

    public static ConversionMarkerClassDefinition? GetConversionMarkerClassDefinitionFromAttribute(GeneratorSyntaxContext context)
    {
     //   var voSyntaxInformation = (TypeDeclarationSyntax) context.Node;

        var semanticModel = context.SemanticModel;

        ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(context.Node)!;

        var voSymbolInformation = (INamedTypeSymbol) declaredSymbol;

        var attributeData = TryGetMarkerAttributes(voSymbolInformation).ToImmutableArray();

        if (attributeData.Length == 0) return null;

        return new ConversionMarkerClassDefinition(
            voSymbolInformation,
            attributeData.Select(a => BuildConverterMarkerDefinitionsFromAttributes.TryBuild(a, voSymbolInformation)));
    }

    public static bool IsTarget(SyntaxNode node) => 
        node is TypeDeclarationSyntax { AttributeLists.Count: > 0 };
}