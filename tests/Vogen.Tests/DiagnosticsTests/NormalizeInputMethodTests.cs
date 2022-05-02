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

    [Fact]
    public void NormalizeInput_FailsWithParameterOfWrongType()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static int NormalizeInput(bool value) => 0;
}";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();
        
        using (new AssertionScope())
        {
            diagnostic.Id.Should().Be("VOG016");
            diagnostic.ToString().Should().Be("(7,5): error VOG016: NormalizeInput must accept one parameter of the same type as the underlying type");
        }

    }

    [Fact]
    public void NormalizeInput_FailsWithReturnOfWrongType()
    {
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static bool NormalizeInput(int value) => 0;
}";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();
        
        using (new AssertionScope())
        {
            diagnostic.Id.Should().Be("VOG015");
            diagnostic.ToString().Should().Be("(7,5): error VOG015: NormalizeInput must return the same underlying type");
        }

    }
}