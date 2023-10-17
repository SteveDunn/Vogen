using ConsumerTests.StringComparisons;
using FluentAssertions.Execution;

namespace ConsumerTests.StringComparisons;

public class ForRecordClasses
{
    [Fact]
    public void Comparison_omitted_but_specifying_own_comparer()
    {
        using var _ = new AssertionScope();

        StringVo_RecordClass_NothingSpecified left = StringVo_RecordClass_NothingSpecified.From("abc");
        StringVo_RecordClass_NothingSpecified right = StringVo_RecordClass_NothingSpecified.From("abc");

        left.Should().Be(right);
        (left == right).Should().BeTrue();

        IEqualityComparer<StringVo_RecordClass_NothingSpecified> myComparer = new MyComparer_Class();
        left.Equals(right, myComparer).Should().BeTrue();
        Dictionary<StringVo_RecordClass_NothingSpecified, int> d = new(myComparer);
        d.Add(StringVo_RecordClass_NothingSpecified.From("one"), 1);
        d.ContainsKey(StringVo_RecordClass_NothingSpecified.From("ONE")).Should().BeTrue();
    }

    [Fact]
    public void OrdinalIgnoreCase()
    {
        using var _ = new AssertionScope();

        StringVo_RecordClass left = StringVo_RecordClass.From("abc");
        StringVo_RecordClass right = StringVo_RecordClass.From("AbC");

        left.Equals(right, StringVo_RecordClass.Comparers.OrdinalIgnoreCase).Should().BeTrue();
        (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }

#if NET7_0_OR_GREATER
    [Fact]
    public void OrdinalIgnoreCase_Generic()
    {
        using var _ = new AssertionScope();

        var left = StringVo_RecordClass_Generic.From("abc");
        var right = StringVo_RecordClass_Generic.From("AbC");

        var comparer = StringVo_RecordClass_Generic.Comparers.OrdinalIgnoreCase;

        left.Equals(right, comparer).Should().BeTrue();

        (comparer.GetHashCode(left) == comparer.GetHashCode(right)).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }
#endif

    [Fact]
    public void OrdinalIgnoreCase_in_a_dictionary()
    {
        Dictionary<StringVo_RecordClass, int> d = new(StringVo_RecordClass.Comparers.OrdinalIgnoreCase);

        using var _ = new AssertionScope();

        StringVo_RecordClass key1Lower = StringVo_RecordClass.From("abc");
        StringVo_RecordClass key2Mixed = StringVo_RecordClass.From("AbC");

        d.Add(key1Lower, 1);
        d.Should().ContainKey(key2Mixed);
    }

    public class MyComparer_Class : IEqualityComparer<StringVo_RecordClass_NothingSpecified>
    {
        public bool Equals(StringVo_RecordClass_NothingSpecified? x, StringVo_RecordClass_NothingSpecified? y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x?.Value, y?.Value);

        public int GetHashCode(StringVo_RecordClass_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }

    public class MyComparer_RecordClass : IEqualityComparer<StringVo_RecordClass_NothingSpecified>
    {
        public bool Equals(StringVo_RecordClass_NothingSpecified? x, StringVo_RecordClass_NothingSpecified? y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x?.Value, y?.Value);

        public int GetHashCode(StringVo_RecordClass_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}