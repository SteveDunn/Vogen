using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests.GlobalConfig;

public class HappyTests
{
    [Fact]
    public async Task Type_override()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(underlyingType: typeof(float))]


                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                     }
                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Exception_override()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(throws: typeof(Whatever.MyValidationException))]

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                         private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                     }

                     public class MyValidationException : Exception
                     {
                         public MyValidationException(string message) : base(message) { }
                     }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Conversion_and_exceptions_override()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]


                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                         private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                     }


                     public class MyValidationException : Exception
                     {
                         public MyValidationException(string message) : base(message) { }
                     }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Conversion_and_exceptions_override_exception_in_different_namespace()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever2.MyValidationException))]

                     namespace Whatever
                     {
                         [ValueObject]
                         public partial struct CustomerId
                         {
                             private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                         }
                     }

                     namespace Whatever2
                     {
                         public class MyValidationException : Exception
                         {
                             public MyValidationException(string message) : base(message) { }
                         }
                     }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task DeserializationStrictness_override()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, deserializationStrictness: DeserializationStrictness.AllowAnything)]


                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId { }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Override_all()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws:typeof(Whatever.MyValidationException), deserializationStrictness: DeserializationStrictness.AllowAnything)]

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId
                     {
                         private static Validation Validate(string value) => value.Length > 0 ? Validation.Ok : Validation.Invalid("xxxx");
                     }

                     public class MyValidationException : Exception
                     {
                         public MyValidationException(string message) : base(message) { }
                     }

                     """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();
        return;

        static void Validate(ImmutableArray<Diagnostic> diagnostics) => diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Does_not_error_that_conversions_is_unspecified_if_omitted()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     [assembly: VogenDefaults()]

                     namespace Whatever;

                     [ValueObject]
                     public partial class CustomerId { }

                     """;

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
}
