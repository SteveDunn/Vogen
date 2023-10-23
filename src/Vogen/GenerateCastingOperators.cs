using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateCastingOperators
{
    public static string Generate(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;
        var itemUnderlyingType = item.UnderlyingTypeFullName;

        StringBuilder sb = new();

        if (item.FromPrimitiveCastOperator == CastOperator.Explicit)
        {
            sb.AppendLine(
                $$"""
                          public static explicit operator {{className}}({{itemUnderlyingType}} value) => From(value);
                  """);
        }

        if (item.ToPrimitiveCastOperator == CastOperator.Explicit)
        {
            sb.AppendLine(
                $$"""
                          public static explicit operator {{itemUnderlyingType}}({{className}} value) => value.Value;
                  """);
        }

        if (item.ToPrimitiveCastOperator == CastOperator.Implicit)
        {
            sb.AppendLine(
                $$"""
                          public static implicit operator {{itemUnderlyingType}}({{className}} vo) => vo._value;
                  """);
        }

        if (item.FromPrimitiveCastOperator == CastOperator.Implicit)
        {
            sb.AppendLine(
                $$"""
                          public static implicit operator {{className}}({{itemUnderlyingType}} value) {
                            return new {{className}}(value);
                  }
                  """);
        }

        if (sb.Length == 0)
        {
            sb.AppendLine();
        }

        return sb.ToString();
    }
}