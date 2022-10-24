using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            yield return new[] {"public partial class"};
            yield return new[] {"public sealed partial class"};
            yield return new[] {"public partial struct"};
            yield return new[] {"public readonly partial struct"};
            yield return new[] {"public sealed partial record class"};
            yield return new[] {"public sealed partial record"};
            yield return new[] {"public partial record struct"};
            yield return new[] {"public readonly partial record struct"};

            yield return new[] {"internal partial class"};
            yield return new[] {"internal sealed partial class"};
            yield return new[] {"internal partial struct"};
            yield return new[] {"internal readonly partial struct"};
            yield return new[] {"internal sealed partial record class"};
            yield return new[] {"internal sealed partial record"};
            yield return new[] {"internal partial record struct"};
            yield return new[] {"internal readonly partial record struct"};
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public void NormalizeInput_FailsWithParameterOfWrongType(string type)
    {
        var source = BuildSource(type);

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
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
    public void NormalizeInput_FailsWithReturnOfWrongType(string type)
    {
        var source = BuildSource(type);

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
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
public {type} CustomerId
{{
    private static bool NormalizeInput(int value) => 0;
}}";
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public void NormalizeInput_FailsWhenNonStatic(string type)
    {
        var source = BuildSource(type);

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
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
}