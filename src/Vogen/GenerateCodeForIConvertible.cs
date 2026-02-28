using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Vogen.Generators;

namespace Vogen;

public static class GenerateCodeForIConvertible
{
    /// <summary>
    /// When the underlying primitive implements <see cref="System.IConvertible"/>,
    /// we add the interface to the generated value object so that
    /// Convert.ChangeType and friends will work.
    /// </summary>
    public static string GenerateInterfaceDefinitionsIfNeeded(string precedingText, GenerationParameters parameters)
    {
        if (parameters.WorkItem.UnderlyingType.Implements(parameters.VogenKnownSymbols.IConvertible))
        {
            return precedingText + " global::System.IConvertible";
        }

        return string.Empty;
    }

    /// <summary>
    /// Some of the IConvertible methods are already hoisted elsewhere (ToString with
    /// IFormatProvider is produced by the IFormattable logic) so we skip any method
    /// named "ToString" and delegate the rest back to the underlying primitive.
    /// </summary>
    public static string GenerateAnyHoistedIConvertibleMethods(GenerationParameters parameters)
    {
        StringBuilder sb = new();
        sb.AppendLine("#nullable disable");

        var primitiveSymbol = parameters.WorkItem.UnderlyingType;
        var wrapperSymbol = parameters.WorkItem.WrapperType;
        var interfaceSymbol = parameters.VogenKnownSymbols.IConvertible;

        if (interfaceSymbol is null)
            return string.Empty;

        if (!primitiveSymbol.Implements(interfaceSymbol))
            return string.Empty;
        if (wrapperSymbol.Implements(interfaceSymbol))
            return string.Empty;

        foreach (var eachInterfaceMethod in interfaceSymbol.GetJustMethods())
        {
            // skip ToString because we already hoist ToString overloads elsewhere
            if (eachInterfaceMethod.Name == "ToString")
                continue;

            var primitiveMethod = primitiveSymbol.FindImplementationForInterfaceMember(eachInterfaceMethod) as IMethodSymbol;
            if (primitiveMethod is null)
                continue;

            string hoistMethodFromPrimitive = Hoisting.HoistMethodFromPrimitive(
                primitiveMethod,
                interfaceSymbol,
                (valueAccessor, parameterNames) =>
                {
                    return $"return IsInitialized() ? {valueAccessor}.{eachInterfaceMethod.Name}({parameterNames}) : default;";
                });

            sb.AppendLine(hoistMethodFromPrimitive);
        }

        sb.AppendLine("#nullable restore");
        return sb.ToString();
    }
}