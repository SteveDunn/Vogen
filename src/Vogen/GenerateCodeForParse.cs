using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal static class GenerateCodeForParse
{
    public static string GenerateAnyHoistedParseMethods(VoWorkItem item)
    {
        if (item.ParsingInformation.UnderlyingIsAString)
        {
            StringBuilder sb = new StringBuilder();
            
            bool configSaysGenerate = item.Config.ParsableForStrings is ParsableForStrings.GenerateMethodsAndInterface or ParsableForStrings.GenerateMethods;
            if (configSaysGenerate)
            {
                sb.Append(BuildParseWithFormatProviderMethodForAString(item));
            }

            return sb.ToString();
        }
        
        if (item.Config.ParsableForPrimitives is not (ParsableForPrimitives.HoistMethods or ParsableForPrimitives.HoistMethodsAndInterfaces))
        {
            return string.Empty;
        }

        INamedTypeSymbol primitiveSymbol = item.UnderlyingType;

        try
        {
            var methodsToWrite = 
                FilterOutUserSuppliedParseMethods(item.ParsingInformation.ParseMethodsOnThePrimitive, item.UserProvidedOverloads.ParseMethods).ToList();
                
            if (methodsToWrite.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
                
            foreach (var eachSymbol in methodsToWrite)
            {
                BuildParseMethod(eachSymbol, sb, item);
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);
        }

        static IEnumerable<IMethodSymbol> FilterOutUserSuppliedParseMethods(
            List<IMethodSymbol> parseMethodsOnThePrimitive, 
            UserProvidedParseMethods parseMethodsOnTheVo)
        {
            foreach (var eachParseMethodOnThePrimitive in parseMethodsOnThePrimitive)
            {
                if (!parseMethodsOnTheVo.Contains(eachParseMethodOnThePrimitive))
                {
                    yield return eachParseMethodOnThePrimitive;
                }
            }
        }
    }

    private static string BuildParseWithFormatProviderMethodForAString(VoWorkItem item)
    {
        var matches = GetUserSuppliedParseWithFormatProviderMethodMatches(item);
        
        if (matches == UserSuppliedParseMethods.ExactMatch)
        {
            return string.Empty;
        }

        bool needsExplicitImplementation = matches == UserSuppliedParseMethods.PartialMatchesButDifferentReturnType;
        
        string methodDecl = needsExplicitImplementation ? $"static {item.VoTypeName} global::System.IParsable<{item.VoTypeName}>.Parse" : $"public static {item.VoTypeName} Parse";
        
        return @$"
    /// <summary>
    /// </summary>
    /// <returns>
    /// The value created via the <see cref=""From(global::System.String)""/> method.
    /// </returns>
    /// <exception cref=""ValueObjectValidationException"">Thrown when the value can be parsed, but is not valid.</exception>
    {methodDecl}(global::System.String s, global::System.IFormatProvider provider) {{
        return From(s);
    }}";
    }

    private static UserSuppliedParseMethods GetUserSuppliedParseWithFormatProviderMethodMatches(VoWorkItem item)
    {
        var allStaticParseMethods = item.UserProvidedOverloads.ParseMethods.Where(
            m => m.IsStatic &&
                 m.Parameters.Length == 2 &&
                 m.Parameters[0].Type.SpecialType == SpecialType.System_String &&
                 SymbolEqualityComparer.Default.Equals(m.Parameters[1].Type, item.ParsingInformation.IFormatProviderType)).ToList();

        if (allStaticParseMethods.Count == 0) return UserSuppliedParseMethods.NoMatches;

        return allStaticParseMethods.Any(m => SymbolEqualityComparer.Default.Equals(m.ReturnType, item.WrapperType))
            ? UserSuppliedParseMethods.ExactMatch
            : UserSuppliedParseMethods.PartialMatchesButDifferentReturnType;
    }

    [Flags]
    enum UserSuppliedParseMethods
    {
        NoMatches,
        ExactMatch,
        PartialMatchesButDifferentReturnType
    }

    private static void BuildParseMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
    {
        string parameters = BuildParametersForParse(methodSymbol);
        string parameterNames = BuildParameterNamesForParse(methodSymbol);
        string staticOrNot = methodSymbol.IsStatic ? "static " : string.Empty;

        var inheritDocRef = methodSymbol.ToString()!
            .Replace("<", "{")
            .Replace(">", "}");
            
        var ret =
            @$"
    /// <inheritdoc cref=""{inheritDocRef}""/>
    /// <summary>
    /// </summary>
    /// <returns>
    /// The value created by calling the Parse method method on the primitive.
    /// </returns>
    /// <exception cref=""ValueObjectValidationException"">Thrown when the value can be parsed, but is not valid.</exception>
    public {staticOrNot}{item.VoTypeName} Parse({parameters}) {{
        var r = {item.UnderlyingTypeFullName}.Parse({parameterNames});
        return From(r);
    }}";

        sb.AppendLine(ret);
    }

    private static string BuildParametersForParse(IMethodSymbol methodSymbol)
    {
        var parametersLength = methodSymbol.Parameters.Length;
        
        List<string> l = new(parametersLength);

        for (var index = 0; index < parametersLength; index++)
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

    private static string BuildParameterNamesForParse(IMethodSymbol methodSymbol)
    {
        var parametersLength = methodSymbol.Parameters.Length;
        
        List<string> l = new(parametersLength);
        
        for (var index = 0; index < parametersLength; index++)
        {
            var eachParameter = methodSymbol.Parameters[index];
            l.Add($"{eachParameter.Name}");
        }

        return string.Join(", ", l);
    }
}