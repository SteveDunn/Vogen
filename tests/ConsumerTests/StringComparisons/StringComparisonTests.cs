using ConsumerTests.StringComparisons;
using FluentAssertions.Execution;

namespace ConsumerTests.Instances;

public class Tests
{
    [Fact]
    public void NoComparison()
    {
        using var _ = new AssertionScope();

        VoWithNothingSpecified left = VoWithNothingSpecified.From("abc");
        VoWithNothingSpecified right = VoWithNothingSpecified.From("abc");

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase()
    {
        using var _ = new AssertionScope();

        VoOrdinalIgnoreCase left = VoOrdinalIgnoreCase.From("abc");
        VoOrdinalIgnoreCase right = VoOrdinalIgnoreCase.From("AbC");

        left.Equals(right).Should().BeTrue();

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase_as_object()
    {
        using var _ = new AssertionScope();

        VoOrdinalIgnoreCase left = VoOrdinalIgnoreCase.From("abc");
        VoOrdinalIgnoreCase right = VoOrdinalIgnoreCase.From("AbC");

        left.Equals((object)right).Should().BeTrue();

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }

    [Fact]
    [UseCulture("tr-TR")]
    public void Different_cultures()
    {
        // i = İ => true
        var left =  VoCurrentCultureIgnoreCase.From("i");
        var right = VoCurrentCultureIgnoreCase.From("\u0130");

        left.Equals(right).Should().BeTrue();
    }
}