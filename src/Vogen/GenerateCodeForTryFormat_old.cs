using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForTryFormat_old
{
    public static string GenerateAnyHoistedTryFormatMethods(VoWorkItem item)
    {
        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;

        try
        {
            List<IMethodSymbol> methodsToWrite = FilterOutUserSuppliedMethods(
                item.FormattableInformation.TryFormatMethodsOnThePrimitive,
                item.UserProvidedOverloads.TryFormatMethods,
                item).ToList();

            if (methodsToWrite.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
                
            foreach (var eachSymbol in methodsToWrite)
            {
                BuildHoistedTryFormatMethod(eachSymbol, sb, item);
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);
        }
        
        // We're given the TryFormat methods on the primitive, and we want to filter out
        // any matching methods that the user has supplied.
        // We want to include any TryFormat methods that result in either the underlying primitive,
        // or the wrapper.
        static IEnumerable<IMethodSymbol> FilterOutUserSuppliedMethods(
            List<IMethodSymbol> tryFormatMethodsOnThePrimitive,
            UserProvidedTryFormatMethods parseMethodsOnTheVo, 
            VoWorkItem vo)
        {
            foreach (var eachTryFormatMethodOnThePrimitive in tryFormatMethodsOnThePrimitive)
            {
                if (!parseMethodsOnTheVo.Contains(eachTryFormatMethodOnThePrimitive, vo))
                {
                    yield return eachTryFormatMethodOnThePrimitive;
                }
            }
        }
    }

    private static bool UserHasSuppliedTheirOwn(VoWorkItem item) =>
        item.UserProvidedOverloads.TryFormatMethods.Any(
            m => m.IsStatic &&
                 m.Parameters.Length == 4 &&
                 m.ReturnType.SpecialType == SpecialType.System_Boolean &&
                 m.Parameters[0].Type.SpecialType == SpecialType.System_String &&
                 m.Parameters[1].RefKind == RefKind.Out && m.Parameters[1].Type.SpecialType == SpecialType.System_Char &&
                 m.Parameters[2].Type.SpecialType == SpecialType.System_String &&
                 SymbolEqualityComparer.Default.Equals(m.Parameters[3].Type, item.ParsingInformation.IFormatProviderType));

    private static void BuildHoistedTryFormatMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
    {
        string parameters = BuildParametersForTryFormat(methodSymbol, item);
        string parameterNames = BuildParameterNamesForTryFormat(methodSymbol);
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
                  public {{staticOrNot}}global::System.Boolean TryFormat({{parameters}}) 
                  {
                      return IsInitialized() ? Value.TryFormat(parameters) : false;
                  }
              """;

        sb.AppendLine(ret);
    }

    private static string BuildParametersForTryFormat(IMethodSymbol methodSymbol, VoWorkItem item)
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

    private static string BuildParameterNamesForTryFormat(IMethodSymbol methodSymbol)
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