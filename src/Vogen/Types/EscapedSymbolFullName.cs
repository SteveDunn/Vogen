using Microsoft.CodeAnalysis;

namespace Vogen.Types;

internal class EscapedSymbolFullName
{
    public EscapedSymbolFullName(INamedTypeSymbol symbol)
    {
        OriginalName = symbol.FullName();

        Value = Util.EscapeKeywordsIfRequired(symbol.FullName());
    }

    public string OriginalName { get; }
    public string Value { get; }
    
    public static implicit operator string(EscapedSymbolFullName name) => name.Value;
    public override string ToString() => Value;
}