using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForToString
{
    public static string GenerateForAClass(VoWorkItem item) => GenerateToString(item, false);
    
    public static string GenerateForAStruct(VoWorkItem item) => GenerateToString(item, true);

    private static string GenerateToString(VoWorkItem item, bool isReadOnly)
    {
        return GenerateAnyHoistedToStringMethods(item);
//         if (item.UserProvidedOverloads.ToStringOverloads.Count == 0)
//         {
//             return string.Empty;
//         }
//         
//         string ro = isReadOnly ? " readonly" : string.Empty;
//         
//         // we don't consider nullability here as *our* ToString never returns null, and we are free to 'narrow'
//         // the signature (going from nullable to non-nullable): https://github.com/dotnet/coreclr/pull/23466#discussion_r269822099
//         
//         return $"""
//                 /// <summary>Returns the string representation of the underlying <see cref="{Util.EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}" />.</summary>
//                 public{ro} override global::System.String ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";
//                 """;
    }
    
    
    public static string GenerateAnyHoistedToStringMethods(VoWorkItem item)
    {
        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;

        try
        {
            List<IMethodSymbol> methodsToWrite = FilterOutUserSuppliedMethods(
                item.ToStringInformation.ToStringMethodsOnThePrimitive,
                item.UserProvidedOverloads.ToStringOverloads,
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
            List<IMethodSymbol> methodsOnThePrimitive,
            UserProvidedToStringMethods methodsOnTheWrapper, 
            VoWorkItem vo)
        {
            foreach (var eachParseMethodOnThePrimitive in methodsOnThePrimitive)
            {
                if (!methodsOnTheWrapper.Contains(eachParseMethodOnThePrimitive, vo))
                {
                    yield return eachParseMethodOnThePrimitive;
                }
            }
        }
    }
    
    private static void BuildHoistedTryParseMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
    {
        string parameters = BuildParameters(methodSymbol, item);
        string parameterNames = BuildParameterNames(methodSymbol);
        string overrideText = methodSymbol.Parameters.Length == 0 ? "override " : string.Empty;

        var inheritDocRef = methodSymbol.ToString()!
            .Replace("<", "{")
            .Replace(">", "}");
            
        var ret =
            $$"""
              
                  /// <inheritdoc cref="{{inheritDocRef}}"/>
                  /// <summary>Returns the string representation of the underlying <see cref="{{Util.EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}}" />.</summary>
                  /// <returns>
                  /// Returns the string representation of the underlying <see cref="{{Util.EscapeTypeNameForTripleSlashComment(item.UnderlyingType)}}" />.
                  /// </returns>
                  public {{overrideText}}global::System.String ToString({{parameters}}) => IsInitialized() ? Value.ToString({{parameterNames}}) ?? "" : "[UNINITIALIZED]"; 
              """;

        sb.AppendLine(ret);
    }
    
    private static string BuildParameters(IMethodSymbol methodSymbol, VoWorkItem item)
    {
        List<string> l = new();

        for (var index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            IParameterSymbol eachParameter = methodSymbol.Parameters[index];
                
            string refKind = BuildRefKind(eachParameter.RefKind);

            string type = eachParameter.Type.ToDisplayString(
                item.Nullable.IsEnabled ? DisplayFormats.SymbolFormatWhenNullabilityIsOn : DisplayFormats.SymbolFormatWhenNullabilityIsOff);

            string name = Util.EscapeIfRequired(eachParameter.Name);

            l.Add($"{refKind}{type} {name}");
        }

        return string.Join(", ", l);
    }

    private static string BuildRefKind(RefKind refKind) =>
        refKind switch
        {
            RefKind.In => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            _ => ""
        };

    private static string BuildParameterNames(IMethodSymbol methodSymbol)
    {
        List<string> l = new();
        for (var index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            var eachParameter = methodSymbol.Parameters[index];
            l.Add($"{eachParameter.Name}");
        }

        return string.Join(", ", l);
    }

    
}