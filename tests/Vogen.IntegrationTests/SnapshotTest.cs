using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests;

[UsesVerify] 
public class ValueObjectGeneratorTests
{
    private readonly ITestOutputHelper _output;

    public ValueObjectGeneratorTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public Task Partial_struct_created_successfully()
    {
        // The source code to test
        var source = @"using Vogen;
namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task No_namespace()
    {
        // The source code to test
        var source = @"using Vogen;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public void No_creation_using_new()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}

var c = new CustomerId();
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(10,13): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void No_creation_using_implicit_new()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}

CustomerId c = new();
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(10,1): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void No_defaulting_parameters_using_default()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
}

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
        diagnostic.ToString().Should().Be("(16,1): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public Task Produces_instances()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
[Instance(name: ""Unspecified"", value: -1)]
[Instance(name: ""Unspecified1"", value: -2)]
[Instance(name: ""Unspecified2"", value: -3)]
[Instance(name: ""Unspecified3"", value: -4)]
[Instance(name: ""Cust42"", value: 42)]
public partial struct CustomerId
{
}
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Validation_with_PacalCased_validate_method()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Validation_with_camelCased_validate_method()
    {
        // The source code to test
        var source = @"using Vogen;

namespace Whatever;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}