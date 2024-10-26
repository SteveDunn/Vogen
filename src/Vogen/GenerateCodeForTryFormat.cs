using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Vogen.Generators;

namespace Vogen;

public static class GenerateCodeForTryFormat
{
    private static bool DoesPubliclyImplementGenericInterface(INamedTypeSymbol underlyingType, INamedTypeSymbol? openGeneric)
    {
        INamedTypeSymbol? closedGeneric = openGeneric?.Construct(underlyingType);
        return MethodDiscovery.DoesPrimitivePubliclyImplementThisInterface(underlyingType, closedGeneric);
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
        if (interfaceSymbol is null) return;

        var primitiveSymbol = parameters.WorkItem.UnderlyingType;
        var wrapperSymbol = parameters.WorkItem.WrapperType;

        if (!primitiveSymbol.AllInterfaces.Contains(interfaceSymbol)) return;
        if (wrapperSymbol.AllInterfaces.Contains(interfaceSymbol)) return;

        var methodsOnInterface = interfaceSymbol.GetMembers().OfType<IMethodSymbol>();

        foreach (var eachInterfaceMethod in methodsOnInterface)
        {
            var primitiveMethod = primitiveSymbol.FindImplementationForInterfaceMember(eachInterfaceMethod) as IMethodSymbol;
        
            if (primitiveMethod is null)
            {
                continue;
            }

            sb.AppendLine(HoistMethodFromPrimitive(primitiveMethod, interfaceSymbol));
        }
    }

    public static string HoistMethodFromPrimitive(IMethodSymbol methodSymbol, INamedTypeSymbol interfaceSymbol)
    {
        var sb = new StringBuilder();
        
        bool isExplicitInterfaceImplementation = methodSymbol.IsImplementationOfAnyExplicitInterfaceMember();
        
        var inheritDocRef = isExplicitInterfaceImplementation ? interfaceSymbol.ToString() : methodSymbol.ToString()!
            .Replace("<", "{")
            .Replace(">", "}");

        sb.AppendLine($"""/// <inheritdoc cref="{inheritDocRef}"/>""");

        // Append method attributes, ignoring NullableAttribute
        foreach (var attribute in methodSymbol.GetAttributes().Where(a => !a.AttributeClass!.Name.Contains("NullableAttribute")))
        {
            sb.AppendLine($"[{attribute}]");
        }
        
        string accessibility = isExplicitInterfaceImplementation ? "" : "public";

        // Append return type and method name
        sb.Append($"{accessibility} {methodSymbol.ReturnType} {methodSymbol.Name}(");

        // Append parameters with attributes, types, and modifiers
        var parameters = new List<string>();
        List<ParameterRefAndName> nameAndRefKinds = new();
        foreach (var eachParam in methodSymbol.Parameters)
        {
            var parameterBuilder = new StringBuilder();

            // Append parameter attributes, ignoring NullableAttribute
            foreach (var attr in eachParam.GetAttributes().Where(a => !a.AttributeClass!.Name.Contains("NullableAttribute")))
            {
                parameterBuilder.Append($"[{attr}] ");
            }

            string refKind = GetRefKind(eachParam);

            // Append parameter type and name
            var parameterName = eachParam.Name;
            parameterBuilder.Append($"{refKind} {eachParam.Type} {parameterName}");
            nameAndRefKinds.Add(new(parameterName, refKind));
            parameters.Add(parameterBuilder.ToString());
        }

        sb.Append(string.Join(", ", parameters));
        sb.Append(")");

        string parameterNames = string.Join(", ", nameAndRefKinds.Select(x => $"{x.RefKind} {x.Name}"));

        string valueAccessor = isExplicitInterfaceImplementation ? $"(Value as {interfaceSymbol.FullName()})" : "Value";

        string body = $$"""
                        {
                            {{InitializeAnyOutParameters(nameAndRefKinds)}}
                            
                            return IsInitialized() ? {{valueAccessor}}.TryFormat({{parameterNames}}) : false;
                        }
                        """;

        sb.AppendLine(body);
        sb.AppendLine();

        return sb.ToString();
    }

    private static string InitializeAnyOutParameters(List<ParameterRefAndName> nameAndRefKinds)
    {
        StringBuilder osb = new();
        var outs = nameAndRefKinds.Where(p => p.RefKind == "out");
        foreach (var each in outs)
        {
            osb.AppendLine($"{each.Name} = default;");
        }

        return osb.ToString();
    }

    private static string GetRefKind(IParameterSymbol eachParam)
    {
        string refKind = "";

        // Append parameter modifiers (ref, out, in, params)
        if (eachParam.RefKind != RefKind.None)
        {
            refKind = $"{eachParam.RefKind.ToString().ToLower()}";
        }

        if (eachParam.IsParams)
        {
            refKind = "params";
        }

        return refKind;
    }

    record ParameterRefAndName(string Name, string RefKind);

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
        for (var index = 0; index < methodSymbol.Parameters.Length - 1; index++)
        {
            var eachParameter = methodSymbol.Parameters[index];
            l.Add($"{eachParameter.Name}");
        }

        return string.Join(", ", l);
    }
}