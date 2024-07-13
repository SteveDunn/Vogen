using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Vogen;

namespace AnalyzerTests
{
        public class InstanceFieldTests
    {
        public class When_values_cannot_be_converted_to_their_underlying_types
        {
            [Fact]
            public async Task Malformed_float_causes_compilation_error()
            {
                string declaration = $@"using System;
  [ValueObject(underlyingType: typeof(float))]
  [Instance(name: ""Invalid"", value: ""1.23x"")]
  public partial class MyInstanceTests {{ }}";
                var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

                await new TestRunner<ValueObjectGenerator>()
                    .WithSource(source)
                    .ValidateWith(Validate)
                    .RunOnAllFrameworks();

                void Validate(ImmutableArray<Diagnostic> diagnostics)
                {
#if NET7_0_OR_GREATER
            diagnostics.Single().GetMessage().Should().Be(
                "MyInstanceTests cannot be converted. Instance value named Invalid has an attribute with a 'System.String' of '1.23x' which cannot be converted to the underlying type of 'System.Single' - The input string '1.23x' was not in a correct format.");
#else
                    diagnostics.Single().GetMessage().Should().Be(
                        "MyInstanceTests cannot be converted. Instance value named Invalid has an attribute with a 'System.String' of '1.23x' which cannot be converted to the underlying type of 'System.Single' - Input string was not in a correct format.");

#endif
                    diagnostics.Single().Id.Should().Be("VOG023");
                }
            }

            [Fact]
            public async Task Malformed_datetime_causes_compilation_error()
            {
                var source = @"
using Vogen;
using System;
namespace Whatever
{
    [ValueObject(underlyingType: typeof(DateTime))]
    [Instance(name: ""Invalid"", value: ""x2022-13-99"")]
    public partial class MyInstanceTests { }
}";

                await new TestRunner<ValueObjectGenerator>()
                    .WithSource(source)
                    .ValidateWith(Validate)
                    .RunOnAllFrameworks();

                void Validate(ImmutableArray<Diagnostic> diagnostics)
                {
                    diagnostics.Should().HaveCount(1);
                    diagnostics.Single().GetMessage().Should().Contain(
                        "MyInstanceTests cannot be converted. Instance value named Invalid has an attribute with a 'System.String' of 'x2022-13-99' which cannot be converted to the underlying type of 'System.DateTime'");

                    diagnostics.Single().Id.Should().Be("VOG023");
                }
            }

            [Fact]
            public async Task Malformed_DateTimeOffset_causes_compilation_error()
            {
                var source = @"
using Vogen;
using System;
namespace Whatever
{
    [ValueObject(underlyingType: typeof(DateTimeOffset))]
    [Instance(name: ""Invalid"", value: ""x2022-13-99"")]
    public partial class MyInstanceTests { }
}";

                await new TestRunner<ValueObjectGenerator>()
                    .WithSource(source)
                    .ValidateWith(Validate)
                    .RunOnAllFrameworks();

                void Validate(ImmutableArray<Diagnostic> diagnostics)
                {
                    diagnostics.Should().HaveCount(1);
                    diagnostics.Single().GetMessage().Should().Contain(
                        "MyInstanceTests cannot be converted. Instance value named Invalid has an attribute with a 'System.String' of 'x2022-13-99' which cannot be converted to the underlying type of 'System.DateTimeOffset'");

                    diagnostics.Single().Id.Should().Be("VOG023");
                }
            }
        }
    }
}