using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents an instance of an EfCoreConverter attribute on the source method.
/// </summary>
/// <param name="VoSymbol">The symbol for the value object being referenced. In effect, the generic value of the attribute.</param>
/// <param name="UnderlyingType">The symbol for the underlying type that is represented by the value object.</param>
/// <param name="SourceType">The symbol for the method that contains the `EfCoreConverter` attribute(s).</param>
internal record class EfCoreConverterSpec(INamedTypeSymbol VoSymbol, INamedTypeSymbol UnderlyingType, INamedTypeSymbol SourceType);