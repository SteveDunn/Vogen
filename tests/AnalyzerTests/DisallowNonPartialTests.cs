using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class DisallowNonPartialTests
{
    [Theory]
    [InlineData("abstract class")]
    [InlineData("abstract record class")]
    public async Task Disallows_non_partial_types(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
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
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG021");
            diagnostic.ToString().Should()
                .Match("*: error VOG021: Type CustomerId is decorated as a Value Object and should be in a partial type.");
        }
    }
}