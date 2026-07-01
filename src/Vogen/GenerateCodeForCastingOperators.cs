using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators;

namespace Vogen;

public static class GenerateCodeForCastingOperators
{
    public static string GenerateImplementations(GenerationParameters p, TypeDeclarationSyntax tds)
    {
        var item = p.WorkItem;
        var wrapper = tds.Identifier;
        var primitive = item.UnderlyingTypeFullNameWithGlobalAlias;

        string primitiveBang = item.Nullable.BangForUnderlying;

        StringBuilder sb = new();

        var config = item.Config;
        var sag = config.StaticAbstractsGeneration;

        if (config.FromPrimitiveCasting == CastOperator.Explicit || sag.HasFlag(StaticAbstractsGeneration.ExplicitCastFromPrimitive))
        {
            sb.AppendLine($"public static explicit operator {wrapper}({primitive} value) => From(value);");
        }

        // Null-propagation applies to the explicit cast only, for reference-type wrappers in a
        // nullable-enabled context. Unspecified (the default) propagates for reference-type underlyings
        // only - value-type underlyings are left alone so `(int)vo` doesn't change to `int?`. Generate
        // propagates for all underlyings; Omit never does.
        bool nullPropagateExplicit =
            item.IsTheWrapperAReferenceType
            && item.Nullable.IsEnabled
            && config.NullPropagatingToPrimitiveCasts switch
            {
                NullPropagatingToPrimitiveCasts.Omit => false,
                NullPropagatingToPrimitiveCasts.Generate => true,
                _ => !item.IsTheUnderlyingAValueType
            };

        // Generate the call to the Value property so that it throws if uninitialized.
        if (config.ToPrimitiveCasting == CastOperator.Explicit || sag.HasFlag(StaticAbstractsGeneration.ExplicitCastToPrimitive))
        {
            // Don't propagate when the cast is part of the static-abstract IVogen interface - the
            // nullable signature can't satisfy the declared `operator TPrimitive(TSelf)`.
            if (nullPropagateExplicit && !sag.HasFlag(StaticAbstractsGeneration.ExplicitCastToPrimitive))
            {
                sb.AppendLine("[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(value))]");
                sb.AppendLine($"public static explicit operator {primitive}?({wrapper}? value) => value is null ? null : value.Value;");
            }
            else
            {
                sb.AppendLine($"public static explicit operator {primitive}({wrapper} value) => value.Value;");
            }
        }

        // Generate the call to the _value field so that it doesn't throw if uninitialized.
        if (config.ToPrimitiveCasting == CastOperator.Implicit || sag.HasFlag(StaticAbstractsGeneration.ImplicitCastToPrimitive))
        {
            sb.AppendLine($"public static implicit operator {primitive}({wrapper} vo) => vo._value{primitiveBang};");
        }

        if (config.FromPrimitiveCasting == CastOperator.Implicit || sag.HasFlag(StaticAbstractsGeneration.ImplicitCastFromPrimitive))
        {
            if (item.NormalizeInputMethod is not null)
            {
                sb.AppendLine($$"""
                                public static implicit operator {{wrapper}}({{primitive}} value) 
                                {
                                    return new {{wrapper}}({{wrapper}}.NormalizeInput(value));
                                }
                                """);

            }
            else
            {
                sb.AppendLine($$"""
                                public static implicit operator {{wrapper}}({{primitive}} value) 
                                {
                                  return new {{wrapper}}(value);
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