using Microsoft.CodeAnalysis;
using System.Linq;
using System.Reflection;

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
            .SingleOrDefault(x => x.IsPartialDefinition());

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

file static class PropertySymbolExtensions
{
    private static PropertyInfo? _isPartialDefinitionProperty = typeof(IPropertySymbol).GetProperty("IsPartialDefinition");

    // IPropertySymbol.IsPartialDefinition property is available since Roslyn 4.12.2
    // We cannot know in advance the Roslyn version used by the user.
    // This extension method tries to retrieve the property and, if missing, returns a fallback value.
    public static bool IsPartialDefinition(this IPropertySymbol propertySymbol) =>
        (bool?)_isPartialDefinitionProperty?.GetValue(propertySymbol) ?? false;
}