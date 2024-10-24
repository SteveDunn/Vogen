using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForToString
{
    public static string GenerateForAClass(VoWorkItem item) => GenerateAnyHoistedToStringMethods(item, false);

    public static string GenerateForAStruct(VoWorkItem item) => GenerateAnyHoistedToStringMethods(item, true);


    private static string GenerateAnyHoistedToStringMethods(VoWorkItem item, bool isReadOnly)
    {
        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;

        try
        {
            List<IMethodSymbol> methodsToWrite = FilterOutUserSuppliedMethods(
                item.ToStringInformation.ToStringMethodsOnThePrimitive,
                item.UserProvidedOverloads.ToStringOverloads,
                item).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var eachSymbol in methodsToWrite)
            {
                BuildHoistedToStringMethod(eachSymbol, sb, item);
            }

            bool hasDefaultToStringMethod = HasParameterlessMethod(methodsToWrite) ||
                                            HasParameterlessMethod(item.UserProvidedOverloads.ToStringOverloads);

            if (!hasDefaultToStringMethod)
            {
                sb.Append(GenerateDefaultToString(item, isReadOnly));
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);
        }

        static bool HasParameterlessMethod(IEnumerable<IMethodSymbol> methods) => methods.Any(m => m.Parameters.Length == 0);

        // We're given the ToString methods on the primitive, and we want to filter out
        // any matching methods that the user has supplied.
        // We want to include any ToString methods that result in either the underlying primitive,
        // or the wrapper.
        static IEnumerable<IMethodSymbol> FilterOutUserSuppliedMethods(
            List<IMethodSymbol> methodsOnThePrimitive,
            UserProvidedToStringMethods methodsOnTheWrapper,
            VoWorkItem vo)
        {
            foreach (var eachMethod in methodsOnThePrimitive)
            {
                if (!methodsOnTheWrapper.Contains(eachMethod, vo))
                {
                    yield return eachMethod;
                }
            }
        }
    }

    private static string GenerateDefaultToString(VoWorkItem item, bool isReadOnly)
    {
        string ro = isReadOnly ? " readonly" : string.Empty;

        // we don't consider nullability here as *our* ToString never returns null, and we are free to 'narrow'
        // the signature (going from nullable to non-nullable): https://github.com/dotnet/coreclr/pull/23466#discussion_r269822099

        return $$"""
                 {{GenerateComment(item, string.Empty)}}
                 public{{ro}} override global::System.String ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";
                 """;
    }

    private static string GenerateComment(VoWorkItem item, string parameterTypes = "")
    {
        var crefContent = GenerateCrefForUnderlyingType(item.UnderlyingType);
        
        if (item.ToStringInformation.UnderlyingTypeHasADefaultToStringMethod)
        {
            return $$"""
                     /// <inheritdoc cref="{{crefContent}}.ToString({{parameterTypes}})" />
                     """;
        }
        else
        {
            return $$"""
                         /// <summary>
                         /// Returns the wrapped primitive's ToString representation.
                         /// </summary>
                         /// <returns>
                         /// If this instance hasn't been initialised, it will return "[UNINITIALIZED]". Otherwise the wrapped primitive's ToString representation.
                         /// </returns>
                     """;
        }
        
    }

    private static string GenerateCrefForUnderlyingType(INamedTypeSymbol underlyingType) =>
        Util.EscapeTypeNameForTripleSlashComment(underlyingType).Replace(" ", "");

    private static void BuildHoistedToStringMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
    {
        string parameters = BuildParameters(methodSymbol, item);
        string parameterNames = BuildParameterNames(methodSymbol);
        string parameterTypes = BuildParameterTypes(methodSymbol);
        string overrideText = methodSymbol.Parameters.Length == 0 ? "override " : string.Empty;

        var ret =
            $$"""
              
                  {{GenerateComment(item, parameterTypes)}}
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

            string attrs = string.Empty;
            var parameterAttributes = eachParameter.GetAttributes();
            if (parameterAttributes.Length > 0)
            {
                StringBuilder sb2 = new StringBuilder();

                foreach (AttributeData eachAttr in parameterAttributes)
                {
                    if (eachAttr.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.NullableAttribute") continue;

                    var attributeText = eachAttr.ToString();
                    if (!string.IsNullOrEmpty(attributeText)) sb2.Append($"[{attributeText}]");
                }

                attrs = sb2.ToString();
            }

            l.Add($"{attrs}{refKind}{type} {name}");
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

    private static string BuildParameterTypes(IMethodSymbol methodSymbol)
    {
        List<string> l = new();
        for (var index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            var eachParameter = methodSymbol.Parameters[index];
            l.Add($"{eachParameter.Type}");
        }

        return string.Join(", ", l);
    }
}