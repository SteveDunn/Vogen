namespace ConsumerTests.IComparableTests;
[ValueObject(typeof(DateOnly))]
public partial struct DOS1 { }

[ValueObject(typeof(int))]
public partial struct S1 { }

[ValueObject]
public partial struct S2 { }

[ValueObject(typeof(int))]
public partial record struct RS1 { }

[ValueObject]
public partial record struct RS2 { }

[ValueObject(typeof(int))]
public partial record struct RC1 { }

[ValueObject]
public partial record struct RC2 { }

[ValueObject(typeof(int))]
public partial class C1 { }

[ValueObject]
public partial class C2 { }

public class IComparableTests
{
    public class StructTests
    {
        [Fact]
        public void Underlying_type_of_DateOnly_means_the_vo_is_IComparable()
        {
            var first = new DateOnly(2020, 1, 1);
            var second = new DateOnly(2020, 1, 2);
            var third = new DateOnly(2020, 1, 3);

            var l = new List<DOS1>(new[] { DOS1.From(first), DOS1.From(third), DOS1.From(second) });
            l[0].Value.Should().Be(first);
            l[1].Value.Should().Be(third);
            l[2].Value.Should().Be(second);

            l.Sort();

            l[0].Value.Should().Be(first);
            l[1].Value.Should().Be(second);
            l[2].Value.Should().Be(third);
        }

        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable()
        {
            List<int> l = new List<int>(new[] {1, 3, 2});
            l.Sort();

            l[0].Should().Be(1);
            l[1].Should().Be(2);
            l[2].Should().Be(3);

            var l2 = new List<S1>(new[] {S1.From(1), S1.From(3), S1.From(2)});
            l2[0].Value.Should().Be(1);
            l2[1].Value.Should().Be(3);
            l2[2].Value.Should().Be(2);

            l2.Sort();

            l2[0].Value.Should().Be(1);
            l2[1].Value.Should().Be(2);
            l2[2].Value.Should().Be(3);
        }

        [Fact]
        public void Underlying_type_of_default_int_means_the_vo_is_IComparable()
        {
            var l2 = new List<S2>(new[] { S2.From(1), S2.From(3), S2.From(2)});
            l2[0].Value.Should().Be(1);
            l2[1].Value.Should().Be(3);
            l2[2].Value.Should().Be(2);

            l2.Sort();

            l2[0].Value.Should().Be(1);
            l2[1].Value.Should().Be(2);
            l2[2].Value.Should().Be(3);
        }
    }

    public class RecordStructTests
    {
        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable()
        {
            var l = new List<RS1>(new[] {RS1.From(1), RS1.From(3), RS1.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }

        [Fact]
        public void Underlying_type_of_default_int_means_the_vo_is_IComparable()
        {
            var l = new List<RS2>(new[] { RS2.From(1), RS2.From(3), RS2.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }
    }

    public class RecordClassTests
    {
        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable()
        {
            var l = new List<RC1>(new[] {RC1.From(1), RC1.From(3), RC1.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }

        [Fact]
        public void Underlying_type_of_default_int_means_the_vo_is_IComparable()
        {
            var l = new List<RC2>(new[] { RC2.From(1), RC2.From(3), RC2.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }
    }

    public class ClassTests
    {
        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable()
        {
            var l = new List<C1>(new[] {C1.From(1), C1.From(3), C1.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }

        [Fact]
        public void Underlying_type_of_default_int_means_the_vo_is_IComparable()
        {
            var l = new List<C2>(new[] { C2.From(1), C2.From(3), C2.From(2)});
            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(3);
            l[2].Value.Should().Be(2);

            l.Sort();

            l[0].Value.Should().Be(1);
            l[1].Value.Should().Be(2);
            l[2].Value.Should().Be(3);
        }
    }

    public class ExplicitCompareToCalls
    {
        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable()
        {
            var c1 = C1.From(123);
            var cc1 = C1.From(123);
            c1.CompareTo(cc1).Should().Be(0);
        }

        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable_with_object_version_of_same_type()
        {
            var c1 = C1.From(123);
            var cc1 = C1.From(123);
            c1.CompareTo((object)cc1).Should().Be(0);
        }

        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_not_IComparable_with_object_version_of_different_type()
        {
            var c1 = C1.From(123);
            Action a = () => c1.CompareTo((object)123).Should().Be(0);

            a.Should().ThrowExactly<ArgumentException>().WithMessage("Cannot compare to object as it is not of type C1*");
        }

        [Fact]
        public void Underlying_type_of_int_means_the_vo_is_IComparable_with_object_version_of_null_and_behaves_the_same_way_of_returning_1()
        {
            var c1 = C1.From(123);
            c1.CompareTo((object?)null).Should().Be(1);
        }

        [Fact]
        public void Underlying_type_of_default_int_means_the_vo_is_IComparable()
        {
            var c1 = C2.From(123);
            var cc1 = C2.From(123);
            c1.CompareTo(cc1).Should().Be(0);
        }
    }
        
}