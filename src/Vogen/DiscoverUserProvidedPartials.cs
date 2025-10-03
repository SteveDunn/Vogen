using Microsoft.CodeAnalysis;
using System.Linq;

namespace Vogen;

internal class DiscoverUserProvidedPartials
{
    public static UserProvidedPartials Discover(INamedTypeSymbol vo, INamedTypeSymbol underlyingType) =>
        new()
        {
            PartialValue = TryGetPartialProperty(vo, "Value"),
            PartialFrom = TryGetPartialMethod(vo, "From", [underlyingType]),
            PartialBoolTryFrom = TryGetPartialMethod(vo, "TryFrom", [underlyingType, vo]),
            PartialErrorTryFrom = TryGetPartialMethod(vo, "TryFrom", [underlyingType]),
        };

    private static UserProvidedPartial? TryGetPartialProperty(ITypeSymbol vo, string propertyName)
    {
        var property = vo.GetMembers(propertyName)
            .OfType<IPropertySymbol>()
            .SingleOrDefault(x => x.IsPartialDefinition);

        if (property is null)
            return null;

        return new(property.DeclaredAccessibility);
    }

    private static UserProvidedPartial? TryGetPartialMethod(ITypeSymbol vo, string methodName, ITypeSymbol[] parameterTypes)
    {
        var method = vo.GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .Where(x => x.IsPartialDefinition)
            .SingleOrDefault(x =>
                x.Parameters.Select(y => y.Type).SequenceEqual(parameterTypes, SymbolEqualityComparer.Default)
            );

        if (method is null)
            return null;

        return new(method.DeclaredAccessibility);
    }
}
