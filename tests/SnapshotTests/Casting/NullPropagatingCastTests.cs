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
    public async Task Reference_type_underlying_propagates_null()
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
    public async Task Value_type_underlying_does_not_propagate()
    {
        // Propagating would change the cast result type from int to int?, which stops (int)vo compiling.
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
    public async Task Does_not_propagate_the_implicit_cast()
    {
        // Implicit to-primitive casts are intentionally left unchanged - making them nullable would
        // defeat the `string x = vo;` convenience that opting into an implicit cast asks for.
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>(toPrimitiveCasting: CastOperator.Implicit)]
                     public partial class Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static implicit operator global::System.String(Vo vo) => vo._value");
        generated.Should().NotContain("vo is null ? null");
    }

    [Fact]
    public async Task Struct_wrapper_does_not_propagate()
    {
        var source = """
                     #nullable enable
                     using Vogen;
                     namespace Whatever;

                     [ValueObject<string>]
                     public partial struct Vo;
                     """;

        var generated = await GetGeneratedSource(source);

        using var _ = new AssertionScope();
        generated.Should().Contain("public static explicit operator global::System.String(Vo value) => value.Value;");
        generated.Should().NotContain("value is null ? null : value.Value");
    }

    [Fact]
    public async Task Nullable_disabled_context_does_not_propagate()
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
