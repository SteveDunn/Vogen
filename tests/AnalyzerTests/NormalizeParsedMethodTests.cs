using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class NormalizeParsedMethodTests
{
    public class MyClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["public partial struct"];
            yield return ["public readonly partial struct"];
            yield return ["public partial class"];
            yield return ["public sealed partial class"];
            yield return ["public partial record struct"];
            yield return ["public readonly partial record struct"];
            yield return ["public sealed partial record"];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeParsed_FailsWhenNonStatic(string type)
    {
        var source = BuildSource(type, "private string NormalizeParsed(string inputToParse) => inputToParse;");

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG041");
            diagnostic.ToString().Should().Be("(7,5): error VOG041: NormalizeParsed must be static");
        }
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeParsed_FailsWithParameterOfWrongType(string type)
    {
        var source = BuildSource(type, "private static string NormalizeParsed(int inputToParse) => \"\";");

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG043");
            diagnostic.ToString().Should().Be("(7,5): error VOG043: NormalizeParsed must accept one string parameter");
        }
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeParsed_FailsWithReturnOfWrongType(string type)
    {
        var source = BuildSource(type, "private static int NormalizeParsed(string inputToParse) => 0;");

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG042");
            diagnostic.ToString().Should().Be("(7,5): error VOG042: NormalizeParsed must return a string");
        }
    }

    [Theory]
    [ClassData(typeof(MyClassData))]
    public async Task NormalizeParsed_DoesNotReportWhenWellFormed(string type)
    {
        var source = BuildSource(type, "private static string NormalizeParsed(string inputToParse) => inputToParse;");

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    private static string BuildSource(string type, string method) =>
        $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
{type} CustomerId
{{
    {method}
}}";
}
