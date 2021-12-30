using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.SnapshotTests;

[UsesVerify] 
public class DefaultingTests
{
    private readonly ITestOutputHelper _output;

    public DefaultingTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Disallows_default_parameters()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId { }

public class Foo
{
    public void DoSomething(CustomerId customerId = default) {
    }
}

CustomerId c = new();
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(14,1): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_literal_array_members()
    {
        // The source code to test
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId { }

CustomerId[] customers = new CustomerId[] {CustomerId.From(123), default, CustomerId.From(321) };
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(7,30): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_literal_from_local_function()
    {
        // The source code to test
        var source = @"using Vogen;
var c = GetCustomer();

CustomerId GetCustomer() => default;

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);
        
        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(4,1): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_from_local_function()
    {
        // The source code to test
        var source = @"using Vogen;
var c = GetCustomer();

CustomerId GetCustomer() => default(CustomerId);

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultAnalyzer>(source);
        
        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(4,37): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_from_method()
    {
        // The source code to test
        var source = @"using Vogen;
var c =Foo.GetCustomer();

[ValueObject(typeof(int))]
public partial struct CustomerId { }

class Foo  {
    public static CustomerId GetCustomer() => default;
}
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);
        
        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(8,19): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

}