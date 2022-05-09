using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.Tests.DiagnosticsTests;

[UsesVerify] 
public class NormalizeInputMethodTests
{
    private readonly ITestOutputHelper _output;

    public NormalizeInputMethodTests(ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void NormalizeInput_FailsWithParameterOfWrongType(string type)
    {
        var source = BuildSource(type);

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();
        
        using (new AssertionScope())
        {
            diagnostic.Id.Should().Be("VOG016");
            diagnostic.ToString().Should().Be("(7,5): error VOG016: NormalizeInput must accept one parameter of the same type as the underlying type");
        }

        static string BuildSource(string type) =>
            $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} CustomerId
{{
    private static int NormalizeInput(bool value) => 0;
}}";
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void NormalizeInput_FailsWithReturnOfWrongType(string type)
    {
        var source = BuildSource(type);

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();
        
        using (new AssertionScope())
        {
            diagnostic.Id.Should().Be("VOG015");
            diagnostic.ToString().Should().Be("(7,5): error VOG015: NormalizeInput must return the same underlying type");
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
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void NormalizeInput_FailsWhenNonStatic(string type)
    {
        var source = BuildSource(type);

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();
        
        using (new AssertionScope())
        {
            diagnostic.Id.Should().Be("VOG014");
            diagnostic.ToString().Should().Be("(7,5): error VOG014: NormalizeInput must be static");
        }

        static string BuildSource(string type) =>
            $@"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public {type} CustomerId
{{
    private int NormalizeInput(int value) => 0;
}}";
    }
}