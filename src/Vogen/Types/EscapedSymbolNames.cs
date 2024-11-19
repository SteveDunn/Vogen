using Microsoft.CodeAnalysis;

namespace Vogen.Types;

internal class EscapedSymbolNames
{
    public EscapedSymbolNames(INamedTypeSymbol symbol)
    {
        ShortName = new EscapedSymbolName(symbol);
        FullName = new EscapedSymbolFullName(symbol);
    }

    public EscapedSymbolName ShortName { get; }

    public EscapedSymbolFullName FullName { get; }
}