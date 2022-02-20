using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.DiagnosticsTests.LocalConfig.Sad;

[UsesVerify] 
public class SadTests
{
    private readonly ITestOutputHelper _output;

    public SadTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public Task Missing_any_constructors()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Missing_string_constructor()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(object o) : base(o.ToString() { }
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Missing_public_string_constructor_on_exception()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    private MyValidationException(object o) : base(o.ToString() { } // PRIVATE!
}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Not_an_exception()
    {
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException { } // NOT AN EXCEPTION!
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(2);

        diagnostics.Should().SatisfyRespectively(first =>
            {
                first.Id.Should().Be("VOG012");
                first.ToString().Should().Be("(11,14): error VOG012: MyValidationException must derive from System.Exception");
            },
            second =>
            {
                second.Id.Should().Be("VOG013");
                second.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
            });

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}
