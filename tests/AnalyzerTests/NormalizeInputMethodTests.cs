using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class NormalizeInputMethodTests
{
    public class MyClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["public partial class"];
            yield return ["public sealed partial class"];
            yield return ["public partial struct"];
            yield return ["public readonly partial struct"];
            yield return ["public sealed partial record class"];
            yield return ["public sealed partial record"];
            yield return ["public partial record struct"];
            yield return ["public readonly partial record struct"];

            yield return ["internal partial class"];
            yield return ["internal sealed partial class"];
            yield return ["internal partial struct"];
            yield return ["internal readonly partial struct"];
            yield return ["internal sealed partial record class"];
            yield return ["internal sealed partial record"];
            yield return ["internal partial record struct"];
            yield return ["internal readonly partial record struct"];
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeInput_FailsWithParameterOfWrongType(string type)
    {
        var source = BuildSource(type);

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG016");
            diagnostic.ToString().Should().Be(
                "(7,5): error VOG016: NormalizeInput must accept one parameter of the same type as the underlying type");
        }

        static string BuildSource(string type) =>
            $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
{type} CustomerId
{{
    private static int NormalizeInput(bool value) => 0;
}}";
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeInput_FailsWithReturnOfWrongType(string type)
    {
        var source = BuildSource(type);

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();


            diagnostic.Id.Should().Be("VOG015");
            diagnostic.ToString().Should()
                .Be("(7,5): error VOG015: NormalizeInput must return the same underlying type");
        }

        static string BuildSource(string type) =>
            $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
{type} CustomerId
{{
    private static bool NormalizeInput(int value) => false;
}}";
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeInput_FailsWhenNonStatic(string type)
    {
        var source = BuildSource(type);

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG014");
            diagnostic.ToString().Should().Be("(7,5): error VOG014: NormalizeInput must be static");
        }

        static string BuildSource(string type) =>
            $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
{type} CustomerId
{{
    private int NormalizeInput(int value) => 0;
}}";
    }

    [Fact]
    public async Task NormalizeInput_CopesWhenUnderlyingTypeIsDefaulted()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject]
public partial struct Struct_WithDefaultedUnderlyingType
{
    private static int NormalizeInput(int input) => System.Math.Min(128, input);
}";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Fact]
    public async Task NormalizeInput_CopesWhenReturningFullyQualifiedNames()
    {
        var source = @"
namespace Whatever;

[Vogen.ValueObject]
public partial struct Struct_WithDefaultedUnderlyingType
{
    private static System.Int32 NormalizeInput(int input) => System.Math.Min(128, input);
}";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Fact]
    public async Task NormalizeInput_CopesWhenReceivingFullyQualifiedNames()
    {
        var source = @"
namespace Whatever;

[Vogen.ValueObject]
public partial struct Struct_WithDefaultedUnderlyingType
{
    private static int NormalizeInput(System.Int32 input) => System.Math.Min(128, input);
}";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }
}