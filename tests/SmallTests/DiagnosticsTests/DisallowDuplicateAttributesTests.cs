using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class DisallowDuplicateAttributesTests
{
    [Theory]
    [InlineData("abstract partial class")]
    [InlineData("abstract partial record class")]
    public void Disallows_multiple_value_object_attributes(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
[ValueObject]
public {type} CustomerId {{ }}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using (new AssertionScope())
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG024");
            diagnostic.ToString().Should()
                .Match("* error VOG024: Type CustomerId is decorated as a Value Object but is declared multiple times. Remove the duplicate definition or differentiate with a namespace.");
        }
    }
}