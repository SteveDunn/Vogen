using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class DoNotUseConstructorTests
{
    public class PrimaryConstructorTests
    {
        [Theory]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId(int SomethingElse)
{{
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
        }

        [Theory]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void multiple_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId(int SomethingElse, string Name, int Age)
{{
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
        }

        [Theory]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void empty_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId()
{{
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
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
        public void parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId() {{ }}
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void multiple_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId(int SomethingElse, string Name, int Age) {{ }}
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void empty_parameters_disallowed(string type)
        {
            var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId
{{
    public CustomerId() {{ }}
}}
";

            var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            using var scope = new AssertionScope();
            diagnostic.Id.Should().Be("VOG008");
            diagnostic.ToString().Should().Match(
                "*VOG008: Cannot have user defined constructors, please use the From method for creation.");
        }
    }
}