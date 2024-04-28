using System.Runtime.InteropServices;
using FluentAssertions.Execution;

namespace ConsumerTests.StringComparisons;

public class ForRecordStructs
{
    public class Tests
    {
        [Fact]
        public void Comparison_omitted_but_specifying_own_comparer()
        {
            using var _ = new AssertionScope();

            StringVo_RecordStruct_NothingSpecified left = StringVo_RecordStruct_NothingSpecified.From("abc");
            StringVo_RecordStruct_NothingSpecified right = StringVo_RecordStruct_NothingSpecified.From("abc");

            left.Should().Be(right);
            (left == right).Should().BeTrue();

            var myComparer = new MyComparer();
            left.Equals(right, myComparer).Should().BeTrue();
            Dictionary<StringVo_RecordStruct_NothingSpecified, int> d = new(myComparer);
            d.Add(StringVo_RecordStruct_NothingSpecified.From("one"), 1);
            d.ContainsKey(StringVo_RecordStruct_NothingSpecified.From("ONE")).Should().BeTrue();
        }

        [Fact]
        public void OrdinalIgnoreCase()
        {
            using var _ = new AssertionScope();

            StringVo_RecordStruct left = StringVo_RecordStruct.From("abc");
            StringVo_RecordStruct right = StringVo_RecordStruct.From("AbC");

            left.Equals(right, StringVo_RecordStruct.Comparers.OrdinalIgnoreCase).Should().BeTrue();
            (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

            left.Should().NotBe(right);
            (left == right).Should().BeFalse();
        }

        [Fact]
        public void OrdinalIgnoreCase_Generic()
        {
            using var _ = new AssertionScope();

            var left = StringVo_RecordStruct_Generic.From("abc");
            var right = StringVo_RecordStruct_Generic.From("AbC");

            var comparer = StringVo_RecordStruct_Generic.Comparers.OrdinalIgnoreCase;

            left.Equals(right, comparer).Should().BeTrue();

            (comparer.GetHashCode(left) == comparer.GetHashCode(right)).Should().BeTrue();

            left.Should().NotBe(right);
            (left == right).Should().BeFalse();
        }

        [Fact]
        public void OrdinalIgnoreCase_in_a_dictionary()
        {
            Dictionary<StringVo_RecordStruct, int> d = new(StringVo_RecordStruct.Comparers.OrdinalIgnoreCase);

            using var _ = new AssertionScope();

            StringVo_RecordStruct key1Lower = StringVo_RecordStruct.From("abc");
            StringVo_RecordStruct key2Mixed = StringVo_RecordStruct.From("AbC");

            d.Add(key1Lower, 1);
            d.Should().ContainKey(key2Mixed);
        }

        [SkippableFact]
        public void Size_is_not_bigger()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            
            var s1 = Marshal.SizeOf<StringVo_RecordStruct_NothingSpecified>();
            var s2 = Marshal.SizeOf<StringVo_RecordStruct>();

            s1.Should().Be(s2);

            var s3 = Marshal.SizeOf<StringVo_ReadOnly_RecordStruct_NothingSpecified>();
            var s4 = Marshal.SizeOf<StringVo_ReadOnly_RecordStruct>();

            s3.Should().Be(s4);
        }

    }

    public class MyComparer : IEqualityComparer<StringVo_RecordStruct_NothingSpecified>
    {
        public bool Equals(StringVo_RecordStruct_NothingSpecified x, StringVo_RecordStruct_NothingSpecified y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);

        public int GetHashCode(StringVo_RecordStruct_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}