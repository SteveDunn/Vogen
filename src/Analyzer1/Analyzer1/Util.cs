using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1;

public static class Util
{

    public static string EscapeIfRequired(string name) =>
        SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
        SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None
            ? "@" + name
            : name;


    public static bool IsVoType(INamedTypeSymbol namedTypeSymbol)
    {
        ImmutableArray<AttributeData> attributes = namedTypeSymbol.GetAttributes();

        if (attributes.Length == 0)
        {
            return false;
        }

        AttributeData? voAttribute = attributes.SingleOrDefault(
            a => a.AttributeClass?.FullName() is "Vogen.ValueObjectAttribute");

        if (voAttribute is null)
        {
            return false;
        }


        return true;
    }
}