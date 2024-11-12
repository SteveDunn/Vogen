using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents a 'marker class', which is a partial class with one or more 'marker attributes' e.g 'EfCoreConverter', or 'MessagePackFormatter', etc.
/// </summary>
internal sealed class MarkerClassDefinition
{
    public INamedTypeSymbol MarkerClassSymbol { get; }

    public MarkerClassDefinition(INamedTypeSymbol markerClassSymbol, IEnumerable<MarkerAttributeDefinition?> markers)
    {
        MarkerClassSymbol = markerClassSymbol;
        Results = markers.Where(r => r is not null)!;
    }

    private IEnumerable<MarkerAttributeDefinition> Results { get; }

    public IEnumerable<Diagnostic> Diagnostics => Results.SelectMany(r => r.Diagnostics);

    public IEnumerable<MarkerAttributeDefinition> AttributeDefinitions => Results;
}