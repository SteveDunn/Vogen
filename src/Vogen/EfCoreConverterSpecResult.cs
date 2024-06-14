using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal sealed class EfCoreConverterSpecResult
{
    public EfCoreConverterSpecResult(EfCoreConverterSpec? spec, IEnumerable<Diagnostic> diagnostics)
    {
        Spec = spec;
        Diagnostics = diagnostics;
    }
    
    public EfCoreConverterSpec? Spec { get;  }

    public IEnumerable<Diagnostic> Diagnostics { get; }

    public static EfCoreConverterSpecResult Null => new(null, []);
    
    public bool HasDiagnostics => Diagnostics.Any();

    public static EfCoreConverterSpecResult Error(Diagnostic diag) => new(null, [diag]);

    public static EfCoreConverterSpecResult Ok(INamedTypeSymbol voSymbol, INamedTypeSymbol underlyingType, INamedTypeSymbol symbol) =>
        new(spec: new EfCoreConverterSpec(voSymbol, underlyingType, symbol), diagnostics: []);
}