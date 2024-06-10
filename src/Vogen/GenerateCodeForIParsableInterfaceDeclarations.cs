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

        bool isUnderlyingAString = item.ParsingInformation.UnderlyingIsAString;
        
        if (isUnderlyingAString && item.Config.ParsableForStrings != ParsableForStrings.GenerateMethodsAndInterface)
        {
            return string.Empty;
        }

        if (!isUnderlyingAString)
        {
            if (item.Config.ParsableForPrimitives != ParsableForPrimitives.HoistMethodsAndInterfaces)
            {
                return string.Empty;
            }

            if (parsingInformation.PrimitiveHasNoParseOrTryParseMethods)
            {
                return string.Empty;
            }
        }
        
        StringBuilder sb = new StringBuilder();

        IEnumerable<string> interfaces = isUnderlyingAString
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