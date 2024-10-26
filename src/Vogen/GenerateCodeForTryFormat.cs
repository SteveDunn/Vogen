using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Vogen.Generators;

namespace Vogen;

public static class GenerateCodeForTryFormat
{
    public static string GenerateInterfaceDefinitionsIfNeeded(string precedingText, GenerationParameters parameters)
    {
        StringBuilder sb = new StringBuilder();

        if(parameters.WorkItem.UnderlyingType.Implements(parameters.VogenKnownSymbols.IFormattable))
        {
            sb.Append($"{precedingText} global::System.IFormattable");
        }
        
        if(parameters.WorkItem.UnderlyingType.Implements(parameters.VogenKnownSymbols.ISpanFormattable))
        {
            sb.Append($"{precedingText} global::System.ISpanFormattable");
        }

        if(parameters.WorkItem.UnderlyingType.Implements(parameters.VogenKnownSymbols.IUtf8SpanFormattable))
        {
            sb.Append($"{precedingText} global::System.IUtf8SpanFormattable");
        }

        return sb.ToString() ;
    }

    public static string GenerateAnyHoistedTryFormatMethods(GenerationParameters parameters)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("#nullable disable");

        BuildFor(sb, parameters, parameters.VogenKnownSymbols.ISpanFormattable);
        BuildFor(sb, parameters, parameters.VogenKnownSymbols.IUtf8SpanFormattable);
        
        sb.AppendLine("#nullable restore");

        // don't worry about IFormattable, as that is a `ToString` that is hoisted separately.
        return sb.ToString();
    }

    private static void BuildFor(StringBuilder sb, GenerationParameters parameters, INamedTypeSymbol? interfaceSymbol)
    {
        var primitiveSymbol = parameters.WorkItem.UnderlyingType;
        var wrapperSymbol = parameters.WorkItem.WrapperType;
        
        if (!primitiveSymbol.Implements(interfaceSymbol)) return;
        if (wrapperSymbol.Implements(interfaceSymbol)) return;

        foreach (var eachInterfaceMethod in interfaceSymbol.GetJustMethods())
        {
            var primitiveMethod = primitiveSymbol.FindImplementationForInterfaceMember(eachInterfaceMethod) as IMethodSymbol;
        
            if (primitiveMethod is null)
            {
                continue;
            }

            sb.AppendLine(Hoisting.HoistMethodFromPrimitive(primitiveMethod, interfaceSymbol!));
        }
    }
}