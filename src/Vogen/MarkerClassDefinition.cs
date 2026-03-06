using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Extensions;

namespace Vogen;

/// <summary>
/// Represents a 'marker class', which is a partial class with one or more 'marker attributes' e.g 'EfCoreConverter', or 'MessagePackFormatter', etc.
/// </summary>
internal sealed class MarkerClassDefinition
{
    public MarkerClassDefinition(INamedTypeSymbol markerClassSymbol, IEnumerable<MarkerAttributeDefinition> markers)
    {
        MarkerClassSymbol = markerClassSymbol;
        AttributeDefinitions = markers.ToList();
    }

    public INamedTypeSymbol MarkerClassSymbol { get; }

    public IEnumerable<Diagnostic> Diagnostics => AttributeDefinitions.SelectMany(r => r.Diagnostics);

    public IEnumerable<MarkerAttributeDefinition> AttributeDefinitions { get; }
}

internal record MarkerAndAttributes(INamedTypeSymbol Symbol, ImmutableArray<MarkerAttributeDefinition> Attributes);

internal class MarkersCollection
{
    private readonly ImmutableArray<MarkerClassDefinition> _markers;

    public MarkersCollection(ImmutableArray<MarkerClassDefinition> markers) => 
        _markers = markers;

    public ImmutableArray<MarkerAndAttributes> GetByKind(ConversionMarkerKind kind)
    {
        var groupedByName = _markers.GroupBy(mc => mc.MarkerClassSymbol, SymbolEqualityComparer.Default);

        var markerClasses = groupedByName.Select(g =>
            new MarkerAndAttributes(
                g.First().MarkerClassSymbol,
                
                    g.SelectMany(x => x.AttributeDefinitions).DistinctByCompat(x => x.Marker)
                        .Where(a => a.Marker?.Kind == kind).ToImmutableArray()
                ));

        return markerClasses.Where(mc => mc.Attributes.Any()).ToImmutableArray();
    }
}

