using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents the result of an 'EFCore Converter Spec' - that is, a partial class decorated with an `EfCoreConverter`
/// attribute. This is the singular item; a particular attribute on the method, and contains the symbol for the
/// value object being references, a symbol for the underlying type of the value object, and a symbol for the method
/// containing the attribute. 
/// </summary>
internal sealed class EfCoreConverterSpecResult
{
    private EfCoreConverterSpecResult(EfCoreConverterSpec? spec, IEnumerable<Diagnostic> diagnostics)
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