using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Extensions;

public static class AttributeDataExtensions
{
    public static IEnumerable<string> GetExplicitlySpecifiedAttributeConstructorParameters(this AttributeData attributeData)
    {
        var ctor = attributeData.AttributeConstructor;
        if (ctor is null)
        {
            yield break;
        }

        var parameters = ctor.Parameters;

        // Need the syntax of the specific attribute application.
        var attrSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
        var argList = attrSyntax?.ArgumentList;

        if (argList is null || argList.Arguments.Count == 0)
        {
            // No arguments written → any optional params are defaults (implicit)
            yield break;
        }

        // First pass: positional constructor arguments (no NameEquals and no NameColon)
        int index = 0; // walks constructor parameters in order
        foreach (var arg in argList.Arguments)
        {
            if (arg.NameEquals is not null)
            {
                // Property/field initializer: `Name = value` → not a ctor parameter
                continue;
            }

            IParameterSymbol parameterSymbol = parameters[index];

            if (arg.NameColon is null)
            {
                // Positional ctor argument → binds to the next parameter position
                if (index < parameters.Length)
                {
                    yield return parameterSymbol.Name;
                    index++;
                }

                // If more positional args than parameters exist, the compiler will error;
                // analyzer can ignore or record as needed.
                continue;
            }

            // NameColon means a named constructor argument: `paramName: expr`
            var name = arg.NameColon.Name.Identifier.ValueText;

            if (Exists(parameters, name))
            {
                yield return name;
            }
        }

        yield break;

        static bool Exists(ImmutableArray<IParameterSymbol> ps, string name) =>
            Enumerable.Any(ps, t => string.Equals(t.Name, name, StringComparison.Ordinal));
    }
}