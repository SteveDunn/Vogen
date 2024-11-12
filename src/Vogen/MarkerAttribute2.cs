using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents a marker for a MessagePack conversion
/// </summary>
/// <param name="ContainerNamespace">The namespace of the containing marker class.</param>
/// <param name="ContainerTypeName">The type name of the containing marker class.</param>
/// <param name="WrapperType">The symbol for the value object wrapper class.</param>
/// <param name="UnderlyingType">The symbol for the underlying primitive type.</param>
/// <param name="WrapperAccessibility">The accessibility (public, internal, etc.) of the generated type.</param>
public record MarkerAttribute2(
    string ContainerNamespace,
    string ContainerTypeName,
    INamedTypeSymbol WrapperType, 
    INamedTypeSymbol UnderlyingType, 
    string WrapperAccessibility)
{
    public static MarkerAttribute2 FromWorkItem(VoWorkItem voWorkItem)
    {
        var isPublic = voWorkItem.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        return new(voWorkItem.FullNamespace, "", voWorkItem.WrapperType, voWorkItem.UnderlyingType, accessor);
    }
}

/// <summary>
/// Represents a marker for a MessagePack conversion
/// </summary>
/// <param name="ContainerNamespace">The namespace of the containing marker class.</param>
/// <param name="WrapperType">The symbol for the value object wrapper class.</param>
/// <param name="UnderlyingType">The symbol for the underlying primitive type.</param>
/// <param name="WrapperAccessibility">The accessibility (public, internal, etc.) of the generated type.</param>
public record MessagePackStandalone(
    string ContainerNamespace,
    INamedTypeSymbol WrapperType, 
    INamedTypeSymbol UnderlyingType, 
    string WrapperAccessibility)
{
    public static MessagePackStandalone FromWorkItem(VoWorkItem voWorkItem)
    {
        var isPublic = voWorkItem.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        return new(voWorkItem.FullNamespace, voWorkItem.WrapperType, voWorkItem.UnderlyingType, accessor);
    }
}