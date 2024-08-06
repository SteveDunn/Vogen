using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests.GlobalConfig;

public class SadTests
{
    [Fact]
    public async Task Missing_any_constructors()
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
            }

            """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(14,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Fact]
    public async Task Missing_string_constructor()
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
    public MyValidationException(object o) : base(o.ToString()) { }
}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(14,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Fact]
    public async Task Missing_public_string_constructor_on_exception()
    {
        const string source = @"using System;
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
    private MyValidationException(object o) : base(o.ToString()) { } // PRIVATE!
}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG013");
            diagnostic.ToString().Should().Be(
                "(14,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
        }
    }

    [Fact]
    public async Task Not_an_exception()
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

public class MyValidationException { } // NOT AN EXCEPTION!
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {

            diagnostics.Should().HaveCount(2);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG012");
                    first.ToString().Should().Be(
                        "(14,14): error VOG012: MyValidationException must derive from System.Exception");
                },
                second =>
                {
                    second.Id.Should().Be("VOG013");
                    second.ToString().Should().Be(
                        "(14,14): error VOG013: MyValidationException must have at least 1 public constructor with 1 parameter of type System.String");
                });
        }
    }

    [Fact]
    public async Task Not_valid_conversion()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(conversions: (Conversions)4242)]

namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {

            diagnostics.Should().HaveCount(1);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG011");
                    first.ToString().Should().Be(
                        "(4,12): error VOG011: The Conversions specified do not match any known conversions - see the Conversions type");
                });
        }
    }

    [Fact]
    public async Task Not_valid_customization()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(customizations: (Customizations)666)]

namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG019");
                    first.ToString().Should().Be(
                        "(4,12): error VOG019: The Customizations specified do not match any known customizations - see the Customizations type");
                });
        }
    }

    [Fact]
    public async Task Not_valid_deserialization_strictness()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(deserializationStrictness: (DeserializationStrictness)666)]

namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {

            diagnostics.Should().HaveCount(1);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG022");
                    first.ToString().Should().Be(
                        "(4,12): error VOG022: The Deserialization Strictness specified does not match any known customizations - see the DeserializationStrictness type for valid values");
                });
        }
    }

    [Fact]
    public async Task Not_valid_customization_or_conversion()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(customizations: (Customizations)666, conversions: (Conversions)4242)]

namespace Whatever;

[ValueObject]
public partial struct CustomerId { }
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(2);

            diagnostics.Should().SatisfyRespectively(
                first =>
                {
                    first.Id.Should().Be("VOG011");
                    first.ToString().Should().Be(
                        "(4,12): error VOG011: The Conversions specified do not match any known conversions - see the Conversions type");
                },
                second =>
                {
                    second.Id.Should().Be("VOG019");
                    second.ToString().Should().Be(
                        "(4,12): error VOG019: The Customizations specified do not match any known customizations - see the Customizations type");
                });
        }
    }
}
