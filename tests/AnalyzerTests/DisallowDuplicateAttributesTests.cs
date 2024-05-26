using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class DisallowDuplicateAttributesTests
{
    [Theory]
    [InlineData("abstract partial class")]
    [InlineData("abstract partial record class")]
    public async Task Disallows_multiple_value_object_attributes(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
[ValueObject]
public {type} CustomerId {{ }}
";
        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("CS0579");
            diagnostic.ToString().Should()
                .Match("* error CS0579: Duplicate 'ValueObject' attribute");
        }
    }
}