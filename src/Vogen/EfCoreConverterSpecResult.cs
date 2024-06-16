using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal sealed class EfCoreConverterSpecResults
{
    public EfCoreConverterSpecResults(IEnumerable<EfCoreConverterSpecResult?> results) => Results = results.Where(r => r is not null)!;

    private IEnumerable<EfCoreConverterSpecResult> Results { get; }

    public IEnumerable<Diagnostic> Diagnostics => Results.SelectMany(r => r.Diagnostics);
    public IEnumerable<EfCoreConverterSpecResult> Specs => Results;
}

internal sealed class EfCoreConverterSpecResult
{
    public EfCoreConverterSpecResult(EfCoreConverterSpec? spec, IEnumerable<Diagnostic> diagnostics)
    {
        Specs = spec;
        Diagnostics = diagnostics.ToList();
    }
    
    public EfCoreConverterSpec? Specs { get;  }

    public List<Diagnostic> Diagnostics { get; }

    public bool HasDiagnostics => Diagnostics.Any();

    public static EfCoreConverterSpecResult Error(Diagnostic diag) => new(null, [diag]);

    public static EfCoreConverterSpecResult Ok(INamedTypeSymbol voSymbol, INamedTypeSymbol underlyingType, INamedTypeSymbol symbol) =>
        new(spec: new EfCoreConverterSpec(voSymbol, underlyingType, symbol), diagnostics: []);
}