using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace MediumTests.DiagnosticsTests.GlobalConfig;

public class HappyTests
{
    [Fact]
    public void Type_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(float))]


namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().BeEmpty();
        }
    }

    [Fact]
    public void Exception_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(throws: typeof(Whatever.MyValidationException))]

namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Fact]
    public void Conversion_and_exceptions_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]


namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}


public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Fact]
    public void DeserializationStrictness_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, deserializationStrictness: DeserializationStrictness.AllowAnything)]


namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(0);
        }
    }

    [Fact]
    public void Override_all()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws:typeof(Whatever.MyValidationException), deserializationStrictness: DeserializationStrictness.AllowAnything))]

namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(string value) => value.Length > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().BeEmpty();
        }
    }
}
