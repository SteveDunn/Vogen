using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.SnapshotTests;

[UsesVerify] 
public class DisallowNewTests
{
    private readonly ITestOutputHelper _output;

    public DisallowNewTests(ITestOutputHelper output) => _output = output;

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
    public void No_creation_using_implicit_new_from_method()
    {
        // The source code to test
        var source = @"using Vogen;

var c =Foo.GetCustomer();

Console.ReadLine();

[ValueObject(typeof(int))]
public partial struct CustomerId { }

class Foo  {
    public static CustomerId GetCustomer() => new();
}

";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(11,19): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_new_from_method()
    {
        // The source code to test
        var source = @"using Vogen;

var c =Foo.GetCustomer();

Console.ReadLine();

[ValueObject(typeof(int))]
public partial struct CustomerId { }

class Foo  {
    public static CustomerId GetCustomer() => new CustomerId();
}

";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(11,51): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_implicit_new_from_local_function()
    {
        // The source code to test
        var source = @"using Vogen;

var c = GetCustomer();
CustomerId GetCustomer() => new();

Console.ReadLine();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(4,1): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_new_from_local_function()
    {
        // The source code to test
        var source = @"using Vogen;

var c = GetCustomer();
CustomerId GetCustomer() => new CustomerId();

Console.ReadLine();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(4,33): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_new_from_func()
    {
        // The source code to test
        var source = @"using Vogen;
Func<CustomerId> f = () => new CustomerId();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(2,32): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_new_from_func2()
    {
        // The source code to test
        var source = @"using Vogen;
Func<int, int, CustomerId, string, CustomerId> f = (a,b,c,d) => new CustomerId();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(2,69): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_new_from_func3()
    {
        // The source code to test
        var source = @"using Vogen;
Func<int, int, CustomerId, string, Task<CustomerId>> f = async (a,b,c, d) => await Task.FromResult(new CustomerId());

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(2,104): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_implicit_new_from_func()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
Func<CustomerId> f = () => new();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(3,28): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_implicit_new_from_func2()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
Func<int, int, CustomerId, string, CustomerId> f = (a,b,c, d) => new();

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(3,66): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_implicit_new_from_func3()
    {
        // The source code to test
        var source = @"using System;
using System.Threading.Tasks;
using Vogen;
Func<int, int, CustomerId, string, Task<CustomerId>> f = async (a,b,c, d) => await Task.FromResult<CustomerId>(new());

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingImplicitNewAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(4,112): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }

    [Fact]
    public void Disallow_implicit_default_as_parameter()
    {
        // The source code to test
        var source = @"using System;
using System.Threading.Tasks;
using Vogen;
Task<CustomerId> t3 = Task.FromResult<CustomerId>(default);

[ValueObject(typeof(int))]
public partial struct CustomerId { }
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<CreationUsingDefaultLiteralAnalyzer>(source);

        _output.WriteLine(output);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG009");
        diagnostic.ToString().Should().Be("(4,51): error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.");
    }

}