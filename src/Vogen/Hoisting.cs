using System.Collections.Generic;
using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class Hoisting
{
    public static string HoistMethodFromPrimitive(IMethodSymbol methodSymbol, INamedTypeSymbol interfaceSymbol)
    {
        var sb = new StringBuilder();

        bool isExplicitInterfaceImplementation = methodSymbol.IsImplementationOfAnyExplicitInterfaceMember();

        var inheritDocRef = isExplicitInterfaceImplementation
            ? interfaceSymbol.ToString()
            : methodSymbol.ToString()!
                .Replace("<", "{")
                .Replace(">", "}");

        sb.AppendLine($"""/// <inheritdoc cref="{inheritDocRef}"/>""");

        // Append method attributes, ignoring NullableAttribute
        foreach (var attribute in methodSymbol.GetAttributesExcludingNullableAttribute())
        {
            sb.AppendLine($"[{attribute}]");
        }

        string accessibility = isExplicitInterfaceImplementation ? "" : "public";

        // Append return type and method name
        sb.Append($"{accessibility} {methodSymbol.ReturnType} {methodSymbol.Name}(");

        // Append parameters with attributes, types, and modifiers
        var parameters = new List<string>();
        List<ParameterNameAndRef> nameAndRefKinds = new();
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

    private static string InitializeAnyOutParameters(List<ParameterNameAndRef> nameAndRefKinds)
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

    record ParameterNameAndRef(string Name, string RefKind);
}