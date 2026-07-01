using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Shared;
using Vogen;

namespace SnapshotTests.Casting;

public class NullPropagatingCastTests
{
    [Fact]
    public async Task Reference_type_underlying_propagates_null_by_default()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String? (Vo? value) => value is null ? null : value.Value;");
        generated.Should().Contain("[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(value))]");
    }

    [Fact]
    public async Task Value_type_underlying_does_not_propagate_by_default()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<int>]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.Int32(Vo value) => value.Value;");
        generated.Should().NotContain("value is null ? null : value.Value");
    }

    [Fact]
    public async Task Value_type_underlying_propagates_when_explicitly_generated()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<int>(nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Generate)]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        generated.Should().Contain("public static explicit operator global::System.Int32? (Vo? value) => value is null ? null : value.Value;");
    }

    [Fact]
    public async Task Reference_type_underlying_can_opt_out_with_omit()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>(nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Omit)]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String(Vo value) => value.Value;");
        generated.Should().NotContain("value is null ? null : value.Value");
    }

    [Fact]
    public async Task Does_not_propagate_the_implicit_cast()
    {
        // Implicit to-primitive casts are intentionally left unchanged - making them nullable would
        // defeat the `string x = vo;` convenience that opting into an implicit cast asks for.
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>(toPrimitiveCasting: CastOperator.Implicit, nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Generate)]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static implicit operator global::System.String(Vo vo) => vo._value");
        generated.Should().NotContain("vo is null ? null");
    }

    [Fact]
    public async Task Struct_wrapper_never_propagates()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>(nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Generate)]
                     public partial struct Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String(Vo value) => value.Value;");
        generated.Should().NotContain("value is null ? null : value.Value");
    }

    [Fact]
    public async Task Nullable_disabled_context_never_propagates()
    {
        var source = """
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        generated.Should().NotContain("value is null ? null : value.Value");
    }

    [Fact]
    public async Task Global_omit_is_overridable_per_value_object()
    {
        var source = """
                     #nullable enable
                     using Vogen;

                     [assembly: VogenDefaults(nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Omit)]

                     namespace Whatever;

                     [ValueObject<string>]
                     public partial class OmittedVo;

                     [ValueObject<string>(nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Generate)]
                     public partial class PropagatingVo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String(OmittedVo value) => value.Value;");
        generated.Should().Contain("public static explicit operator global::System.String? (PropagatingVo? value) => value is null ? null : value.Value;");
    }

    [Fact]
    public async Task Non_generic_attribute_reads_the_option()
    {
        // Omit on a string (reference) underlying proves the value is actually read from the non-generic
        // attribute - the default (Unspecified) would otherwise propagate for a reference underlying.
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject(underlyingType: typeof(string), nullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Omit)]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String(Vo value) => value.Value;");
        generated.Should().NotContain("value is null ? null : value.Value");
    }

    private static async Task<string> GetGeneratedSource(string source)
    {
        (ImmutableArray<Diagnostic> diagnostics, SyntaxTree[] trees) =
            await new ProjectBuilder()
                .WithUserSource(source)
                .WithTargetFramework(TargetFramework.Net9_0)
                .GetGeneratedOutput<ValueObjectGenerator>(ignoreInitialCompilationErrors: false);

        diagnostics.Should().BeEmpty();

        return string.Join("\n", trees.Select(t => t.ToString()));
    }
}
