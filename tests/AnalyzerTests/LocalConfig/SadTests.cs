using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests.LocalConfig;

public class SadTests
{
    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Missing_any_constructors(string type)
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

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Missing_string_constructor(string path)
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
    public MyValidationException(object o) : base(o.ToString()) {{ }}
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Missing_public_string_constructor_on_exception(string type)
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
    private MyValidationException(object o) : base(o.ToString()) {{ }} // PRIVATE!
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Not_an_exception(string type)
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

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(2);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG012");
                    first.ToString().Should().Be(
                        "(11,14): error VOG012: MyValidationException must derive from System.Exception");
                },
                second =>
                {
                    second.Id.Should().Be("VOG013");
                    second.ToString().Should().Be(
                        "(11,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
                });
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Not_valid_conversion(string type)
    {
        var source = $@"using System;
using Vogen;

namespace Whatever;

[ValueObject(conversions: (Conversions)4242)]
public {type} CustomerId {{ }}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG011");
                    first.ToString().Should().Be(
                        "(6,2): error VOG011: The Conversions specified do not match any known conversions - see the Conversions type");
                });
        }
    }

    [Theory]
    [InlineData("[ValueObject(conversions: Conversions.Unspecified)]")]
    [InlineData("[ValueObject<int>(conversions: Conversions.Unspecified)]")]
    [InlineData("[ValueObject(typeof(int), Conversions.Unspecified)]")]
    [InlineData("[ValueObject<int>(Conversions.Unspecified)]")]
    public async Task Cannot_explicitly_use_unspecified(string attribute)
    {
        var source = $$"""
                     using System;
                     using Vogen;

                     namespace Whatever;

                     {{attribute}}
                     public partial class CustomerId { }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG011");
                    first.ToString().Should().Be(
                        "(6,2): error VOG011: The Conversions specified do not match any known conversions - see the Conversions type");
                });
        }
    }
}
