using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents the *definition* of a 'marker attribute, e.g. an 'EFCore marker' or 'message pack marker' - that is, a partial class decorated with an
/// attribute such as `EfCoreConverter`, or 'MemoryPackConverter'. This is the singular item; a particular attribute on the method,
/// and contains the symbol for the value object being referenced, a symbol for the underlying type of the value object,
/// and a symbol for the method containing the attribute. 
/// </summary>
internal sealed class MarkerAttributeDefinition
{
    private MarkerAttributeDefinition(ConversionMarker? marker, IEnumerable<Diagnostic> diagnostics)
    {
        Marker = marker;
        Diagnostics = diagnostics.ToList();
    }
    
    public ConversionMarker? Marker { get;  }

    public List<Diagnostic> Diagnostics { get; }

    public static MarkerAttributeDefinition Error(Diagnostic diag) => new(null, [diag]);

    public static MarkerAttributeDefinition Ok(ConversionMarkerKind kind, INamedTypeSymbol voSymbol, INamedTypeSymbol underlyingTypeSymbol, INamedTypeSymbol sourceSymbol) =>
        new(marker: new ConversionMarker(kind, voSymbol, underlyingTypeSymbol, sourceSymbol), diagnostics: []);
}