using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

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