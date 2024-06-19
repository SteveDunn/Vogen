using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents a collection of results that are all associated with the same source symbol (that is,
/// the method containing the `EfCoreConverter` attributes.
/// </summary>
internal sealed class EfCoreConverterSpecResults
{
    public INamedTypeSymbol? SourceSymbol { get; }

    public EfCoreConverterSpecResults(INamedTypeSymbol? sourceSymbol, IEnumerable<EfCoreConverterSpecResult?> results)
    {
        SourceSymbol = sourceSymbol;
        Results = results.Where(r => r is not null)!;
    }

    private IEnumerable<EfCoreConverterSpecResult> Results { get; }

    public IEnumerable<Diagnostic> Diagnostics => Results.SelectMany(r => r.Diagnostics);
    public IEnumerable<EfCoreConverterSpecResult> Specs => Results;
}

/// <summary>
/// Represents the result of an 'EFCore Converter Spec' - that is, a partial class decorated with an `EfCoreConverter<>`
/// attribute. This is the singular item; a particular attribute on the method, and contains the symbol for the
/// value object being references, a symbol for the underlying type of the value object, and a symbol for the method
/// containing the attribute. 
/// </summary>
internal sealed class EfCoreConverterSpecResult
{
    public EfCoreConverterSpecResult(EfCoreConverterSpec? spec, IEnumerable<Diagnostic> diagnostics)
    {
        Spec = spec;
        Diagnostics = diagnostics.ToList();
    }
    
    public EfCoreConverterSpec? Spec { get;  }

    public List<Diagnostic> Diagnostics { get; }

    public static EfCoreConverterSpecResult Error(Diagnostic diag) => new(null, [diag]);

    public static EfCoreConverterSpecResult Ok(INamedTypeSymbol voSymbol, INamedTypeSymbol underlyingTypeSymbol, INamedTypeSymbol sourceSymbol) =>
        new(spec: new EfCoreConverterSpec(voSymbol, underlyingTypeSymbol, sourceSymbol), diagnostics: []);
}