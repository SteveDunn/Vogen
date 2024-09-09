using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class DoNotUseConstructorTests
{
    public class PrimaryConstructorTests
    {
        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

#pragma warning disable CS9113 // parameter is unread

[ValueObject]
public {type} CustomerId(int SomethingElse)
{{
}}
";

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }

        [Theory]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task multiple_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId(int SomethingElse, string Name, int Age)
{{
}}
";

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }

        [Theory]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task empty_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId()
{{
}}
";

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }
    }

    public class ConstructorTests
    {
        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId() {{ }}
}}
";

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {

                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task multiple_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId(int SomethingElse, string Name, int Age) {{ }}
}}
";

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {

                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public async Task empty_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId() {{ }}
}}
";
            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOnAllFrameworks();

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {

                diagnostics.Should().HaveCount(1);
                Diagnostic diagnostic = diagnostics.Single();

                using var scope = new AssertionScope();
                diagnostic.Id.Should().Be("VOG008");
                diagnostic.ToString().Should().Match(
                    "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
            }
        }
    }
}