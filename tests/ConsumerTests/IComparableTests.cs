using System.Collections.Generic;
using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.IComparableTests
{
    [ValueObject(typeof(int))]
    public partial struct S1 { }

    [ValueObject(typeof(int))]
    public partial record struct RS1 { }

    [ValueObject(typeof(int))]
    public partial record struct RC1 { }

    [ValueObject(typeof(int))]
    public partial class C1 { }

    public class IComparableTests
    {
        public class StructTests
        {
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

                // (C1.From(1) < C1.From(2)).Should().BeTrue();
            }
        }
    }
}
