using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForToString
{
    public static string GenerateNonReadOnly(VoWorkItem item) => GenerateToString(item, false);
    
    public static string GenerateReadOnly(VoWorkItem item) => GenerateToString(item, true);

    private static string GenerateToString(VoWorkItem item, bool isReadOnly)
    {
        if (item.UserProvidedOverloads.ToStringInfo.WasSupplied)
        {
            return string.Empty;
        }
        
        string ro = isReadOnly ? " readonly" : string.Empty;
        
        // we don't consider nullability here as *our* ToString never returns null, and we are free to 'narrow'
        // the signature (going from nullable to non-nullable): https://github.com/dotnet/coreclr/pull/23466#discussion_r269822099
        
        return $"""
                /// <summary>Returns the string representation of the underlying <see cref="{Util.EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}" />.</summary>
                public{ro} override global::System.String ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";
                """;
    }
    
    
    public static string GenerateAnyHoistedToStringMethods(VoWorkItem item)
    {
        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;

        try
        {
            List<IMethodSymbol> methodsToWrite = FilterOutUserSuppliedMethods(
                item.ParsingInformation.TryParseMethodsOnThePrimitive,
                item.UserProvidedOverloads.TryParseMethods,
                item).ToList();

            if (methodsToWrite.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
                
            foreach (var eachSymbol in methodsToWrite)
            {
                BuildHoistedTryParseMethod(eachSymbol, sb, item);
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);
        }
        
        // We're given the ToString methods on the primitive, and we want to filter out
        // any matching methods that the user has supplied.
        // We want to include any TryParse methods that result in either the underlying primitive,
        // or the wrapper.
        static IEnumerable<IMethodSymbol> FilterOutUserSuppliedMethods(
            List<IMethodSymbol> parseMethodsOnThePrimitive,
            UserProvidedTryParseMethods parseMethodsOnTheVo, 
            VoWorkItem vo)
        {
            foreach (var eachParseMethodOnThePrimitive in parseMethodsOnThePrimitive)
            {
                if (!parseMethodsOnTheVo.Contains(eachParseMethodOnThePrimitive, vo))
                {
                    yield return eachParseMethodOnThePrimitive;
                }
            }
        }
    }
    
}