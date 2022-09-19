using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen
{
    internal static class TryParseGeneration
    {
        public static string GenerateTryParseIfNeeded(VoWorkItem item, TypeDeclarationSyntax tds)
        {
            INamedTypeSymbol? primitiveSymbol = item.UnderlyingType;
            try
            {
                if (primitiveSymbol is null) return string.Empty;

                var found = FindMatches(primitiveSymbol).ToList();

                if (found.Count == 0) return string.Empty;

                StringBuilder sb = new StringBuilder();
                
                foreach (var eachSymbol in found)
                {
                    BuildMethod(eachSymbol, sb, item);
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot parse {primitiveSymbol} - {e}", e);

            }
        }

        private static void BuildMethod(IMethodSymbol methodSymbol, StringBuilder sb, VoWorkItem item)
        {
            string parameters = BuildParameters(methodSymbol, item);
            string parameterNames = BuildParameterNames(methodSymbol, item);
            
            var ret =
                @$"/// <inheritdoc />
public static global::System.Boolean TryParse({parameters}, out {item.VoTypeName} result) {{
    if({item.UnderlyingTypeFullName}.TryParse({parameterNames}, out var r)) {{
        result = From(r);
        return true;
    }}

    result = default;
    return false;
}}";

            sb.AppendLine(ret);
        }

        private static string BuildParameters(IMethodSymbol methodSymbol, VoWorkItem item)
        {
            List<string> l = new();
            for (var index = 0; index < methodSymbol.Parameters.Length-1; index++)
            {
                var eachParameter = methodSymbol.Parameters[index];
                l.Add($"{eachParameter} {eachParameter.Name}");
            }

            return string.Join(", ", l);
        }

        private static string BuildParameterNames(IMethodSymbol methodSymbol, VoWorkItem item)
        {
            List<string> l = new();
            for (var index = 0; index < methodSymbol.Parameters.Length-1; index++)
            {
                var eachParameter = methodSymbol.Parameters[index];
                l.Add($"{eachParameter.Name}");
            }

            return string.Join(", ", l);
        }

        private static IEnumerable<IMethodSymbol> FindMatches(INamedTypeSymbol primitiveSymbol)
        {
            ImmutableArray<ISymbol> members = primitiveSymbol.GetMembers("TryParse");

            if (members.Length == 0) yield break;
            
            foreach (ISymbol eachMember in members)
            {
                if (eachMember is IMethodSymbol s)
                {
                    if (!s.IsStatic) continue;
                    var ps = s.GetParameters();
                    //if (ps.Length != 2) continue;

                    if (s.ReturnType.Name != nameof(Boolean)) continue;
                    if (!SymbolEqualityComparer.Default.Equals(ps[ps.Length-1].Type, primitiveSymbol)) continue;

                    //if (ps[0].Type.Name == nameof(ReadOnlySpan<char>) || ps[1].Type.Name == nameof(String))
                    {
                        yield return s;
                      //  break;
                    }
                }
            }
        }
    }
}