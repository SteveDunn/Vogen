using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests.LocalConfig;

public class HappyTests
{
    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Type_override(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(typeof(float))]
public {type} CustomerId {{ }}";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Exception_override(string type)
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
    public MyValidationException(string message) : base(message) {{ }}
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Conversion_override(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.None)]
public {type} CustomerId {{ }}";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Conversion_and_exceptions_override(string type)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]
public {type} CustomerId
{{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}


public class MyValidationException : Exception
{{
    public MyValidationException(string message) : base(message) {{ }}
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Override_global_config_locally(string type)
    {
        var source = $@"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws:typeof(Whatever.MyValidationException))]

namespace Whatever;

[ValueObject(underlyingType:typeof(float))]
public {type} CustomerId
{{
    private static Validation Validate(float value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}}

public class MyValidationException : Exception
{{
    public MyValidationException(string message) : base(message) {{ }}
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().BeEmpty();
        }
    }
}
