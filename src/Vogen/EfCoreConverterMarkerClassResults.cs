using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents a collection of results that are all associated with the same source symbol (that is,
/// the method containing the `EfCoreConverter` attributes.
/// </summary>
internal sealed class EfCoreConverterMarkerClassResults
{
    public INamedTypeSymbol? MarkerSymbol { get; }

    public EfCoreConverterMarkerClassResults(INamedTypeSymbol? markerSymbol, IEnumerable<EfCoreConverterSpecResult?> results)
    {
        MarkerSymbol = markerSymbol;
        Results = results.Where(r => r is not null)!;
    }

    private IEnumerable<EfCoreConverterSpecResult> Results { get; }

    public IEnumerable<Diagnostic> Diagnostics => Results.SelectMany(r => r.Diagnostics);
    public IEnumerable<EfCoreConverterSpecResult> Specs => Results;
}