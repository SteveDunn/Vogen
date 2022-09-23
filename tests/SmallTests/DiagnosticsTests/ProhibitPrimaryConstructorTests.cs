using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen.Analyzers;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class ProhibitPrimaryConstructorTests
{
    [Theory]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Primary_constructors_with_parameters_disallowed(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId(int SomethingElse)
{{
}}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<PrimaryConstructorAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        using var scope = new AssertionScope();
        diagnostic.Id.Should().Be("VOG018");
        diagnostic.ToString().Should().Match("*Type 'CustomerId' cannot have a primary constructor");
    }

    [Theory]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Primary_constructors_with_multiple_parameters_disallowed(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId(int SomethingElse, string Name, int Age)
{{
}}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<PrimaryConstructorAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        using var scope = new AssertionScope();
        diagnostic.Id.Should().Be("VOG018");
        diagnostic.ToString().Should().Match("*Type 'CustomerId' cannot have a primary constructor");
    }

    [Theory]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Primary_constructors_with_empty_parameters_disallowed(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId()
{{
}}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<PrimaryConstructorAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        using var scope = new AssertionScope();
        diagnostic.Id.Should().Be("VOG018");
        diagnostic.ToString().Should().Match("*Type 'CustomerId' cannot have a primary constructor");
    }
}