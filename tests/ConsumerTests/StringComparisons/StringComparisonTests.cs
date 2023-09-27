using ConsumerTests.StringComparisons;
using FluentAssertions.Execution;

namespace ConsumerTests.Instances.StringComparisons;

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
        (left.GetHashCode() == right.GetHashCode()).Should().BeTrue();

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }

    #if NET7_0_OR_GREATER
    [Fact]
    public void OrdinalIgnoreCase_Generic()
    {
        using var _ = new AssertionScope();

        var left = VoOrdinalIgnoreCase_Generic.From("abc");
        var right = VoOrdinalIgnoreCase_Generic.From("AbC");

        left.Equals(right).Should().BeTrue();
        (left.GetHashCode() == right.GetHashCode()).Should().BeTrue();

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }
#endif

    [Fact]
    public void OrdinalIgnoreCase_Struct()
    {
        using var _ = new AssertionScope();

        var left = VoOrdinalIgnoreCase_Struct.From("abc");
        var right = VoOrdinalIgnoreCase_Struct.From("AbC");

        left.Equals(right).Should().BeTrue();
        (left.GetHashCode() == right.GetHashCode()).Should().BeTrue();

        left.Should().Be(right);
        (left == right).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase_in_a_dictionary()
    {
        Dictionary<VoOrdinalIgnoreCase, int> d = new();
        
        using var _ = new AssertionScope();
        
        VoOrdinalIgnoreCase key1Lower = VoOrdinalIgnoreCase.From("abc");
        VoOrdinalIgnoreCase key2Mixed = VoOrdinalIgnoreCase.From("AbC");
        
        d.Add(key1Lower, 1);
        d.Should().ContainKey(key2Mixed);
    }

    [Fact]
    public void OrdinalIgnoreCase_as_object()
    {
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

    // ignored for now - please see https://github.com/SteveDunn/Vogen/wiki/Records
    // [Fact]
    // [UseCulture("tr-TR")]
    // public void Different_cultures_with_records()
    // {
    //     // i = İ => true
    //     var left =  VoRecordCurrentCultureIgnoreCase.From("i");
    //     var right = VoRecordCurrentCultureIgnoreCase.From("\u0130");
    //
    //     left.Equals(right).Should().BeTrue();
    // }
    
    // [Fact]
    // public void RecordOrdinalIgnoreCase_as_object()
    // {
    //     using var _ = new AssertionScope();
    //
    //     var left = VoRecordCurrentCultureIgnoreCase.From("abc");
    //     var right = VoRecordCurrentCultureIgnoreCase.From("AbC");
    //
    //     left.Equals((object)right).Should().BeTrue();
    //
    //     left.Should().Be(right);
    //     (left == right).Should().BeTrue();
    // }
}