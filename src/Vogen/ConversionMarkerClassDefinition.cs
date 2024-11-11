using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents a collection of EFCore converter marker definitions (`EfCoreConverter<>`) that are all associated with the same type symbol
/// (that is, the type containing the `EfCoreConverter` attributes).
/// </summary>
internal sealed class ConversionMarkerClassDefinition
{
    public INamedTypeSymbol? MarkerClassSymbol { get; }

    public ConversionMarkerClassDefinition(INamedTypeSymbol? markerClassSymbol, IEnumerable<ConversionMarkerAttributeDefinition?> markers)
    {
        MarkerClassSymbol = markerClassSymbol;
        Results = markers.Where(r => r is not null)!;
    }

    private IEnumerable<ConversionMarkerAttributeDefinition> Results { get; }

    public IEnumerable<Diagnostic> Diagnostics => Results.SelectMany(r => r.Diagnostics);
    public IEnumerable<ConversionMarkerAttributeDefinition> AttributeDefinitions => Results;
}