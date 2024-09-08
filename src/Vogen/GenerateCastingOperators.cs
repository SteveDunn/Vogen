using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateCastingOperators
{
    public static string GenerateImplementations(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;
        var itemUnderlyingType = item.UnderlyingTypeFullName;

        string primitiveBang = item.Nullable.BangForUnderlying;

        StringBuilder sb = new();

        if (item.Config.FromPrimitiveCasting == CastOperator.Explicit)
        {
            sb.AppendLine($"public static explicit operator {className}({itemUnderlyingType} value) => From(value);");
        }

        // Generate the call to the Value property so that it throws if uninitialized.
        if (item.Config.ToPrimitiveCasting == CastOperator.Explicit)
        {
            sb.AppendLine($"public static explicit operator {itemUnderlyingType}({className} value) => value.Value;");
        }

        // Generate the call to the _value field so that it doesn't throw if uninitialized.
        if (item.Config.ToPrimitiveCasting == CastOperator.Implicit)
        {
            sb.AppendLine($"public static implicit operator {itemUnderlyingType}({className} vo) => vo._value{primitiveBang};");
        }

        if (item.Config.FromPrimitiveCasting == CastOperator.Implicit)
        {
            if (item.NormalizeInputMethod is not null)
            {
                sb.AppendLine($$"""
                                public static implicit operator {{className}}({{itemUnderlyingType}} value) 
                                {
                                    return new {{className}}({{className}}.NormalizeInput(value));
                                }
                                """);

            }
            else
            {
                sb.AppendLine($$"""
                                public static implicit operator {{className}}({{itemUnderlyingType}} value) 
                                {
                                  return new {{className}}(value);
                                }
                                """);
            }
        }

        if (sb.Length == 0)
        {
            sb.AppendLine();
        }

        return sb.ToString();
    }
}