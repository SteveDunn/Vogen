using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;
using Vogen.Tests;
using Xunit;

namespace SmallTests.DiagnosticsTests.LocalConfig;

public class SadTests
{
    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Missing_any_constructors(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public {type} CustomerId
{{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}

public class MyValidationException : Exception {{ }}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Missing_string_constructor(string path)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public {path} CustomerId
{{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}

public class MyValidationException : Exception
{{
    public MyValidationException(object o) : base(o.ToString() {{ }}
}}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Missing_public_string_constructor_on_exception(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public {type} CustomerId
{{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}

public class MyValidationException : Exception
{{
    private MyValidationException(object o) : base(o.ToString() {{ }} // PRIVATE!
}}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG013");
        diagnostic.ToString().Should().Be("(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Not_an_exception(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(throws: typeof(MyValidationException))]
public {type} CustomerId
{{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}

public class MyValidationException {{ }} // NOT AN EXCEPTION!
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

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
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public void Not_valid_conversion(string type)
    {
        var source = $@"using System;
using Vogen;

namespace Whatever;

[ValueObject(conversions: (Conversions)666)]
public {type} CustomerId {{ }}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        diagnostics.Should().SatisfyRespectively(first =>
        {
            first.Id.Should().Be("VOG011");
            first.ToString().Should().Be("(6,2): error VOG011: The Conversions specified do not match any known conversions - see the Conversions type");
        });
    }
}
