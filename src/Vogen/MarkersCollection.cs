using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Vogen.Extensions;

namespace Vogen;

internal class MarkersCollection
{
    private readonly ImmutableArray<IGrouping<ISymbol?, MarkerDiscovery>> _groupedByName;

    public MarkersCollection(ImmutableArray<MarkerDiscovery?> discoveries)
    {
        ImmutableArray<MarkerDiscovery> filtered = discoveries.Where(d => d?.MarkerClassSymbol is not null).ToImmutableArray()!;
        _groupedByName = filtered.GroupBy(mc => mc.MarkerClassSymbol, SymbolEqualityComparer.Default).ToImmutableArray();
    }

    public ImmutableArray<MarkerAndAttributes> GetByKind(ConversionMarkerKind kind)
    {
        var markerClasses = _groupedByName.Select(g =>
            new MarkerAndAttributes(
                g.First().MarkerClassSymbol,
                g.SelectMany(x => x.AttributeDefinitions).DistinctByCompat(x => x.Marker)
                    .Where(a => a.Marker?.Kind == kind).ToImmutableArray()
            ));

        return markerClasses.Where(mc => mc.Attributes.Any()).ToImmutableArray();
    }
}