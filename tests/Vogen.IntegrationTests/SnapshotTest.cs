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
}