using ConsumerTests.StringComparisons;
using FluentAssertions.Execution;

namespace ConsumerTests.StringComparisons;

[Collection("Sequential")]
public class Tests
{
    [UseCulture("tr-TR")]
    [Collection("Sequential")]
    public class LocaleTests
    {
        [Fact]
        public void WithClasses_Different_cultures()
        {
            // i = İ => true
            var left = StringVo_Class.From("i");
            var right = StringVo_Class.From("\u0130");

            left.Equals(right, StringVo_Class.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
        }
        
        [Fact]
        public void WithStructs_Different_cultures()
        {
            // i = İ => true
            var left = StringVo_Struct.From("i");
            var right = StringVo_Struct.From("\u0130");

            left.Equals(right, StringVo_Struct.Comparers.CurrentCultureIgnoreCase).Should().BeTrue();
        }
        
    }

    [Collection("Sequential")]
    public class ClassTests
    {

        [Fact]
        public void Comparison_omitted_but_specifying_own_comparer()
        {
            using var _ = new AssertionScope();

            StringVo_Class_NothingSpecified left = StringVo_Class_NothingSpecified.From("abc");
            StringVo_Class_NothingSpecified right = StringVo_Class_NothingSpecified.From("abc");

            left.Should().Be(right);
            (left == right).Should().BeTrue();

            IEqualityComparer<StringVo_Class_NothingSpecified> myComparer = new MyComparer();
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
        //     var left = VoRecordCurrentCultureIgnoreCase.From("abc");F
        //     var right = VoRecordCurrentCultureIgnoreCase.From("AbC");
        //
        //     left.Equals((object)right).Should().BeTrue();
        //
        //     left.Should().Be(right);
        //     (left == right).Should().BeTrue();
        // }
    }

    [Collection("Sequential")]
    public class StructTests
    {
        [Fact]
        public void NoComparison()
        {
            using var _ = new AssertionScope();

            StringVo_Struct_NothingSpecified left = StringVo_Struct_NothingSpecified.From("abc");
            StringVo_Struct_NothingSpecified right = StringVo_Struct_NothingSpecified.From("abc");

            left.Should().Be(right);
            (left == right).Should().BeTrue();
        }


        [Fact]
        public void OrdinalIgnoreCase()
        {
            using var _ = new AssertionScope();

            StringVo_Struct_NothingSpecified left = StringVo_Struct_NothingSpecified.From("abc");
            StringVo_Struct_NothingSpecified right = StringVo_Struct_NothingSpecified.From("AbC");

            (left.GetHashCode() != right.GetHashCode()).Should().BeTrue();

            left.Should().NotBe(right);
            (left == right).Should().BeFalse();
        }

#if NET7_0_OR_GREATER
    [Fact]
    public void OrdinalIgnoreCase_Generic()
    {
        using var _ = new AssertionScope();

        var left = StringVo_Struct_Generic.From("abc");
        var right = StringVo_Struct_Generic.From("AbC");

        var comparer = StringVo_Struct_Generic.Comparers.OrdinalIgnoreCase;
        
        left.Equals(right, comparer).Should().BeTrue();
        
        (comparer.GetHashCode(left) == comparer.GetHashCode(right)).Should().BeTrue();

        left.Should().NotBe(right);
        (left == right).Should().BeFalse();
    }
#endif

        [Fact]
        public void OrdinalIgnoreCase_in_a_dictionary()
        {
            Dictionary<StringVo_Struct, int> d = new(StringVo_Struct.Comparers.OrdinalIgnoreCase);

            using var _ = new AssertionScope();

            StringVo_Struct key1Lower = StringVo_Struct.From("abc");
            StringVo_Struct key2Mixed = StringVo_Struct.From("AbC");

            d.Add(key1Lower, 1);
            d.Should().ContainKey(key2Mixed);
        }
    }

    public class MyComparer : IEqualityComparer<StringVo_Class_NothingSpecified>
    {
        public bool Equals(StringVo_Class_NothingSpecified? x, StringVo_Class_NothingSpecified? y) =>
            StringComparer.OrdinalIgnoreCase.Equals(x?.Value, y?.Value);

        public int GetHashCode(StringVo_Class_NothingSpecified obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}