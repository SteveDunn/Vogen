using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class DisallowNonPartialTests
{
    [Theory]
    [InlineData("abstract class")]
    [InlineData("abstract record class")]
    public void Disallows_non_partial_types(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public {type} CustomerId {{ }}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using (new AssertionScope())
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG021");
            diagnostic.ToString().Should()
                .Match("*: error VOG021: Type CustomerId is decorated as a Value Object and should be in a partial type.");
        }
    }
}