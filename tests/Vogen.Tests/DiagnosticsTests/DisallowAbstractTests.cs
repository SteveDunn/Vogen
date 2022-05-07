using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Vogen;

namespace Vogen.Tests.DiagnosticsTests;

[UsesVerify] 
public class DisallowAbstractTests
{
    private readonly ITestOutputHelper _output;

    public DisallowAbstractTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Disallows_abstract_value_objects()
    {
        var source = @"using Vogen;

namespace Whatever;

[ValueObject]
public partial abstract class CustomerId { }
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using (new AssertionScope())
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG017");
            diagnostic.ToString().Should()
                .Be("(6,31): error VOG017: Type 'CustomerId' cannot be abstract");
        }
    }

    [Fact]
    public void Disallows_nested_abstract_value_objects()
    {
        var source = @"using Vogen;

namespace Whatever;

public class MyContainer {
    [ValueObject]
    public partial abstract class CustomerId { }
}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using (new AssertionScope())
        {
            diagnostics.Should().HaveCount(2);
            Diagnostic diagnostic = diagnostics.ElementAt(0);

            diagnostic.Id.Should().Be("VOG017");
            diagnostic.ToString().Should()
                .Be("(7,35): error VOG017: Type 'CustomerId' cannot be abstract");

            diagnostic = diagnostics.ElementAt(1);

            diagnostic.Id.Should().Be("VOG001");
            diagnostic.ToString().Should()
                .Be("(7,35): error VOG001: Type 'CustomerId' cannot be nested - remove it from inside MyContainer");
        }
    }
}