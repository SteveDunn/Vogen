using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

public enum ConverterMarkerKind
{
    EFCore,
    MessagePack,
}

/// <summary>
/// Represents the *definition* of a 'marker attribute, e.g. an 'EFCore marker' or 'memory pack marker' - that is, a partial class decorated with an
/// attribute such as `EfCoreConverter`, or 'MemoryPackConverter'. This is the singular item; a particular attribute on the method,
/// and contains the symbol for the value object being referenced, a symbol for the underlying type of the value object,
/// and a symbol for the method containing the attribute. 
/// </summary>
internal sealed class ConversionMarkerAttributeDefinition
{
    private ConversionMarkerAttributeDefinition(ConverterMarker? marker, IEnumerable<Diagnostic> diagnostics)
    {
        Marker = marker;
        Diagnostics = diagnostics.ToList();
    }
    
    public ConverterMarker? Marker { get;  }

    public List<Diagnostic> Diagnostics { get; }

    public static ConversionMarkerAttributeDefinition Error(ConverterMarkerKind efCore, Diagnostic diag) => new(null, [diag]);

    public static ConversionMarkerAttributeDefinition Ok(ConverterMarkerKind kind, INamedTypeSymbol voSymbol, INamedTypeSymbol underlyingTypeSymbol, INamedTypeSymbol sourceSymbol) =>
        new(marker: new ConverterMarker(kind, voSymbol, underlyingTypeSymbol, sourceSymbol), diagnostics: []);
}