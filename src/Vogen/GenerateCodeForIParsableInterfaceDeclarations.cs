using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateCodeForIParsableInterfaceDeclarations
{
    private static readonly string[] _allInterfacesForAString = ["IParsable"]; 
    
    public static string GenerateIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var parsingInformation = item.ParsingInformation;

        if (!parsingInformation.IParsableIsAvailable)
        {
            return string.Empty;
        }
        
        if (parsingInformation is { UnderlyingIsAString: false, PrimitiveHasNoParseOrTryParseMethods: true })
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();

        IEnumerable<string> interfaces = parsingInformation.UnderlyingIsAString
            ? _allInterfacesForAString
            : GetInterfacesImplementedPubliclyOnThePrimitive(item.ParsingInformation);
        
        foreach (var i in interfaces)
        {
            sb.Append($"{precedingText} global::System.{i}<{tds.Identifier}>");
        }
        
        return sb.ToString() ;
    }

    /// <summary>
    /// Gets the interfaces implemented publicly on the primitive. It excludes interfaces that are implemented privately,
    /// for instance, bool implements the static Parse and TryParse methods privately from `IParsable`
    /// </summary>
    /// <param name="parsingInformation"></param>
    /// <returns></returns>
    private static IEnumerable<string> GetInterfacesImplementedPubliclyOnThePrimitive(ParsingInformation parsingInformation)
    {
        if (parsingInformation.UnderlyingDerivesFromIParsable || parsingInformation.UnderlyingIsAString)
        {
            yield return "IParsable"; 
        }

        if (parsingInformation.UnderlyingDerivesFromISpanParsable)
        {
            yield return "ISpanParsable"; 
        }

        if (parsingInformation.UnderlyingDerivesFromIUtf8SpanParsable)
        {
            yield return "IUtf8SpanParsable"; 
        }
    }
    
}