using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class Hoisting
{
    /// <summary>
    /// Generates a string that represents a complete method. The signature is the same as the provided <see cref="methodSymbol"/>.
    /// If the method is an explicit implementation of the <see cref="interfaceSymbol"/>, then it is implemented privately.
    /// The body of the method initialises any out parameters.
    /// It then calls the <see cref="bodyBuilder"/> func to get the implementation.
    /// Any XML documentation is copied.
    /// </summary>
    /// <param name="methodSymbol">The method to copy the signature from.</param>
    /// <param name="interfaceSymbol">The interface</param>
    /// <param name="bodyBuilder"></param>
    /// <param name="emitNullableAware">
    /// When true, parameter types keep their nullable reference type annotations (e.g. <c>IFormatProvider?</c>)
    /// instead of being stripped, for hoists emitted inside a <c>#nullable enable</c> region.
    /// </param>
    /// <returns></returns>
    public static string HoistMethodFromPrimitive(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol interfaceSymbol,
        Func<string, string, string> bodyBuilder,
        bool emitNullableAware = false)
    {
        var sb = new StringBuilder();

        bool isExplicitInterfaceImplementation = methodSymbol.IsImplementationOfAnyExplicitInterfaceMember();

        // In nullable-aware mode the emitted signature must match the interface contract, so
        // parameter nullability comes from the interface method, not from the primitive's
        // implementation (whose parameters may be oblivious in metadata).
        IMethodSymbol? signatureSource = null;
        if (emitNullableAware)
        {
            signatureSource = methodSymbol.ExplicitInterfaceImplementations.FirstOrDefault()
                ?? interfaceSymbol.GetJustMethods().FirstOrDefault(m =>
                    m.Name == methodSymbol.Name && m.Parameters.Length == methodSymbol.Parameters.Length);
        }

        var inheritDocRef = isExplicitInterfaceImplementation
            ? interfaceSymbol.ToString()
            : methodSymbol.OriginalDefinition.ToString()!
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
        string returnTypeText = signatureSource is not null
            ? signatureSource.ReturnType.ToDisplayString(DisplayFormats.SymbolFormatWhenNullabilityIsOn)
            : methodSymbol.ReturnType.ToDisplayString() ?? string.Empty;
        sb.Append($"{accessibility} {returnTypeText} {methodSymbol.Name}(");

        // Append parameters with attributes, types, and modifiers
        var parameters = new List<string>();
        List<ParameterNameAndRef> nameAndRefKinds = new();
        for (int paramIndex = 0; paramIndex < methodSymbol.Parameters.Length; paramIndex++)
        {
            var eachParam = methodSymbol.Parameters[paramIndex];
            var parameterBuilder = new StringBuilder();

            // Append parameter attributes, ignoring NullableAttribute
            foreach (var attr in eachParam.GetAttributes().Where(a => !a.AttributeClass!.Name.Contains("NullableAttribute")))
            {
                parameterBuilder.Append($"[{attr}] ");
            }

            string refKind = GetRefKind(eachParam);

            // Append parameter type and name
            var parameterName = Util.EscapeKeywordsIfRequired(eachParam.Name);

            // In nullable-aware mode the parameter type (including any NRT annotation) comes from the
            // interface contract; otherwise annotations are stripped for emission under #nullable disable.
            ITypeSymbol paramTypeSymbol = eachParam.Type;
            if (emitNullableAware && signatureSource is not null && paramIndex < signatureSource.Parameters.Length)
            {
                paramTypeSymbol = signatureSource.Parameters[paramIndex].Type;
            }

            string typeAsText = paramTypeSymbol.ToDisplayString(
                emitNullableAware ? DisplayFormats.SymbolFormatWhenNullabilityIsOn : DisplayFormats.SymbolFormatWhenNullabilityIsOff);

            //var paramType = eachParam.Type;

            parameterBuilder.Append($"{refKind} {typeAsText} {parameterName}");
            nameAndRefKinds.Add(new(parameterName, refKind));
            parameters.Add(parameterBuilder.ToString());
        }

        sb.Append(string.Join(", ", parameters));
        sb.Append(")");

        string parameterNames = string.Join(", ", nameAndRefKinds.Select(x => $"{x.RefKind} {x.Name}"));

        string valueAccessor = isExplicitInterfaceImplementation
            ? $"(Value as {interfaceSymbol.EscapedFullName()}){(emitNullableAware ? "!" : string.Empty)}"
            : "Value";

        string body = $$"""
                        {
                            {{InitializeAnyOutParameters(nameAndRefKinds)}}
                            
                            {{bodyBuilder(valueAccessor, parameterNames)}}
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