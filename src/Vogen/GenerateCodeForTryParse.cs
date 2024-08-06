using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForTryParse
{

    public static string GenerateAnyHoistedTryParseMethods(VoWorkItem item)
    {
        if (item.ParsingInformation.UnderlyingIsAString)
        {
            if (item.Config.ParsableForStrings is ParsableForStrings.GenerateMethods or ParsableForStrings.GenerateMethodsAndInterface)
            {
                return BuildTryParseMethodForAString(item);
            }
        }

        if (item.Config.ParsableForPrimitives is not (ParsableForPrimitives.HoistMethods or ParsableForPrimitives.HoistMethodsAndInterfaces))
        {
            return string.Empty;
        }
        
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
                BuildTryParseMethod(eachSymbol, sb, item);
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);

        }
        
        // We're given the TryParse methods on the primitive, and we want to filter out
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

    private static string BuildTryParseMethodForAString(VoWorkItem item)
    {
        if (UserHasSuppliedTheirOwn(item))
        {
            return string.Empty;
        }
        
        return $$"""
                 
                     /// <summary>
                     /// </summary>
                     /// <returns>
                     /// True if the value passes any validation (after running any optional normalization).
                     /// </returns>
                     public static global::System.Boolean TryParse(global::System.String s, global::System.IFormatProvider provider, {{Util.GenerateNotNullWhenTrueAttribute()}} out {{item.VoTypeName}} result) 
                     {
                             {{Util.GenerateCallToNormalizeMethodIfNeeded(item, "s")}}
                             {{GenerateCallToValidationIfNeeded(item, "s")}}
                             result = new {{item.VoTypeName}}(s);
                             return true;
                     }
                 """;
    }

    private static bool UserHasSuppliedTheirOwn(VoWorkItem item) =>
        item.UserProvidedOverloads.TryParseMethods.Any(
            m => m.IsStatic && 
                 m.Parameters.Length == 3 && 
                 m.ReturnType.SpecialType == SpecialType.System_Boolean &&
                 m.Parameters[0].Type.SpecialType == SpecialType.System_String &&
                 SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, item.ParsingInformation.IFormatProviderType) &&
                 SymbolEqualityComparer.Default.Equals(m.Parameters[2].Type, item.WrapperType) &&
                 m.Parameters[2].RefKind == RefKind.Out);

    private static void BuildTryParseMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
    {
        string parameters = BuildParametersForTryParse(methodSymbol);
        string parameterNames = BuildParameterNamesForTryParse(methodSymbol);
        string staticOrNot = methodSymbol.IsStatic ? "static " : string.Empty;
        
        var inheritDocRef = methodSymbol.ToString()!
            .Replace("<", "{")
            .Replace(">", "}");
            
        var ret =
            $$"""
              
                  /// <inheritdoc cref="{{inheritDocRef}}"/>
                  /// <summary>
                  /// </summary>
                  /// <returns>
                  /// True if the value could a) be parsed by the underlying type, and b) passes any validation (after running any optional normalization).
                  /// </returns>
                  public {{staticOrNot}}global::System.Boolean TryParse({{parameters}}, {{Util.GenerateNotNullWhenTrueAttribute()}} out {{item.VoTypeName}} result) 
                  {
                      if({{item.UnderlyingTypeFullName}}.TryParse({{parameterNames}}, out var __v)) {
                          {{Util.GenerateCallToNormalizeMethodIfNeeded(item, "__v")}}
                          {{GenerateCallToValidationIfNeeded(item, "__v")}}
                          result = new {{item.VoTypeName}}(__v);
                          return true;
                      }
              
                      result = default;
                      return false;
                  }
              """;

        sb.AppendLine(ret);
    }
    
    private static string GenerateCallToValidationIfNeeded(VoWorkItem workItem, string parameterName)
    {
        if (workItem.ValidateMethod is not null)
        {
            return $$"""
                     var validation = {{workItem.TypeToAugment.Identifier}}.{{workItem.ValidateMethod.Identifier.Value}}({{parameterName}});
                     if (validation != Vogen.Validation.Ok)
                     {
                         result = default;
                         return false;
                     }

                     """;
        }

        return string.Empty;
    }


    private static string BuildParametersForTryParse(IMethodSymbol methodSymbol)
    {
        List<string> l = new();

        for (var index = 0; index < methodSymbol.Parameters.Length-1; index++)
        {
            IParameterSymbol eachParameter = methodSymbol.Parameters[index];
                
            string refKind = BuildRefKind(eachParameter.RefKind);

            string type = eachParameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

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

    private static string BuildParameterNamesForTryParse(IMethodSymbol methodSymbol)
    {
        List<string> l = new();
        for (var index = 0; index < methodSymbol.Parameters.Length-1; index++)
        {
            var eachParameter = methodSymbol.Parameters[index];
            l.Add($"{eachParameter.Name}");
        }

        return string.Join(", ", l);
    }
}