using Microsoft.CodeAnalysis;

namespace Vogen.Types;

internal class EscapedSymbolName
{
    public EscapedSymbolName(INamedTypeSymbol symbol)
    {
        OriginalName = symbol.Name;

        Value = Util.EscapeKeywordsIfRequired(symbol.Name);
    }

    public string OriginalName { get; }
    public string Value { get; }
    
    public static implicit operator string(EscapedSymbolName name) => name.Value;
    public override string ToString() => Value;
}