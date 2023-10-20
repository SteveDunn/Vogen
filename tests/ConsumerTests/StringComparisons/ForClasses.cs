using FluentAssertions.Execution;

namespace ConsumerTests.StringComparisons;

public class ForClasses
{
    [Fact]
    public void Comparison_omitted_but_specifying_own_comparer()
    {
        using var _ = new AssertionScope();

        StringVo_Class_NothingSpecified left = StringVo_Class_NothingSpecified.From("abc");
        StringVo_Class_NothingSpecified right = StringVo_Class_NothingSpecified.From("abc");

        left.Should().Be(right);
        (left == right).Should().BeTrue();

        IEqualityComparer<StringVo_Class_NothingSpecified> myComparer = new MyComparer_Class();
        left.Equals(right, myComparer).Should().BeTrue();
        Dictionary<StringVo_Class_NothingSpecified, int> d = new(myComparer);
        d.Add(StringVo_Class_NothingSpecified.From("one"), 1);
        d.ContainsKey(StringVo_Class_NothingSpecified.From("ONE")).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase()
    {
        using var _ = new AssertionScope();

        StringVo_Class left = StringVo_Class.From("abc");
        StringVo_Class right = StringVo_Class.From("AbC");

        left.Equals(right, StringVo_Class.Comparers.OrdinalIgnoreCase).Should().BeTrue();
        (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }

#if NET7_0_OR_GREATER
    [Fact]
    public void OrdinalIgnoreCase_Generic()
    {
        using var _ = new AssertionScope();

        var left = StringVo_Class_Generic.From("abc");
        var right = StringVo_Class_Generic.From("AbC");

        var comparer = StringVo_Class_Generic.Comparers.OrdinalIgnoreCase;

        left.Equals(right, comparer).Should().BeTrue();

        (comparer.GetHashCode(left) == comparer.GetHashCode(right)).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }
#endif

    [Fact]
    public void OrdinalIgnoreCase_in_a_dictionary()
    {
        Dictionary<StringVo_Class, int> d = new(StringVo_Class.Comparers.OrdinalIgnoreCase);

        using var _ = new AssertionScope();

        StringVo_Class key1Lower = StringVo_Class.From("abc");
        StringVo_Class key2Mixed = StringVo_Class.From("AbC");

        d.Add(key1Lower, 1);
        d.Should().ContainKey(key2Mixed);
    }
    
    public class MyComparer_Class : IEqualityComparer<StringVo_Class_NothingSpecified>
    {
        public bool Equals(StringVo_Class_NothingSpecified? x, StringVo_Class_NothingSpecified? y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x?.Value, y?.Value);

        public int GetHashCode(StringVo_Class_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}
