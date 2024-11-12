using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents an instance of a marker attribute, e.g. `EfCoreConverterAttribute` or a `MemoryPackageConverterAttribute`, and its
/// containing marker class.
/// </summary>
/// <param name="VoSymbol">The symbol for the value object being referenced. In effect, the generic value of the attribute.</param>
/// <param name="UnderlyingTypeSymbol">The symbol for the underlying type that is represented by the value object.</param>
/// <param name="MarkerClassSymbol">The symbol for the marker class that has the marker attribute(s).</param>
internal record ConversionMarker(
    ConversionMarkerKind Kind,
    INamedTypeSymbol VoSymbol,
    INamedTypeSymbol UnderlyingTypeSymbol,
    INamedTypeSymbol MarkerClassSymbol);