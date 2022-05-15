using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Vogen.Analyzers;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.Tests.DiagnosticsTests;

[UsesVerify] 
public class ProhibitPrimaryConstructorTests
{
    private readonly ITestOutputHelper _output;

    public ProhibitPrimaryConstructorTests(ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void No_primary_constructors_with_parameters_allowed(string type)
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
    public void No_primary_constructors_with_empty_parameters(string type)
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