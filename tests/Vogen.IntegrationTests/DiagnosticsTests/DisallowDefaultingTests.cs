using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.SnapshotTests;

[UsesVerify] 
public class DisallowDefaultingTests
{
    private readonly ITestOutputHelper _output;

    public DisallowDefaultingTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Disallows_default_parameters()
    {
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
    public void Allows_non_vo_default_parameters()
    {
        var source = @"using Vogen;
using System.Collections;

namespace Whatever;

public class Foo
{
    public void DoSomething(Hashtable ht = default)
    {
    }
}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Allows_non_vo_default_return_value()
    {
        var source = @"using System.Collections;
var d = GetHashtable();
Hashtable? GetHashtable() => default;
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Disallows_default_literal_array_members()
    {
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
        diagnostic.ToString().Should().Be("(7,66): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_literal_from_local_function()
    {
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
        diagnostic.ToString().Should().Be("(4,29): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_from_local_function()
    {
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
        diagnostic.ToString().Should().Be("(8,47): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_literal_from_func()
    {
        var source = @"using System;
using Vogen;
Func<CustomerId> f = () => default;

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);
        
        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(3,28): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

    [Fact]
    public void Disallows_default_from_func()
    {
        var source = @"using Vogen;
Func<CustomerId> f = () => default(CustomerId);

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultAnalyzer>(source);
        
        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(2,36): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

}