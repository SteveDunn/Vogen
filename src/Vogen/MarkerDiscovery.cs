using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents the discovery of a 'marker class', which is a partial class with one or more 'marker attributes' e.g 'EfCoreConverter', or 'MessagePackFormatter', etc.
/// It may be invalid, in which case it will contain Diagnostics.
/// </summary>
internal sealed class MarkerDiscovery
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="markerClassSymbol"></param>
    /// <param name="markers">A collection of null null <see cref="MarkerPropertiesAndDiagnostics"/></param>
    public MarkerDiscovery(INamedTypeSymbol markerClassSymbol, ImmutableArray<MarkerPropertiesAndDiagnostics> markers)
    {
        MarkerClassSymbol = markerClassSymbol;
        AttributeDefinitions = markers;
        Diagnostics = AttributeDefinitions.SelectMany(r => r.Diagnostics).ToImmutableArray();
    }

    public INamedTypeSymbol MarkerClassSymbol { get; }

    public ImmutableArray<Diagnostic> Diagnostics
    {
        get;
    }

    public ImmutableArray<MarkerPropertiesAndDiagnostics> AttributeDefinitions { get; }
}

internal record MarkerAndAttributes(INamedTypeSymbol Symbol, ImmutableArray<MarkerPropertiesAndDiagnostics> Attributes);