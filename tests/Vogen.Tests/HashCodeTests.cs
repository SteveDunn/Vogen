using System.Collections.Generic;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit;

namespace Vogen.Tests.HashCodes
{
    [ValueObject(typeof(int))]
    public partial struct MyStructInt { }

    [ValueObject(typeof(int))]
    public partial struct MyStructInt2 { }

    [ValueObject(typeof(int))]
    public partial class MyClassInt { }

    [ValueObject(typeof(int))]
    public partial class MyClassInt2 { }
    
    public class HashCodeTests
    {
        public class WithStructs
        {
            [Fact]
            public void Hashing()
            {
                (Age.From(18).GetHashCode() == Age.From(18).GetHashCode()).Should().BeTrue();
                (Age.From(18).GetHashCode() == Age.From(19).GetHashCode()).Should().BeFalse();
                (Age.From(18).GetHashCode() == Score.From(1).GetHashCode()).Should().BeFalse();
                (Age.From(18).GetHashCode() == Score.From(18).GetHashCode()).Should().BeFalse();
            }

            [Fact]
            public void SameStructsHaveSameHashCode()
            {
                MyStructInt.From(0).GetHashCode().Should().Be(MyStructInt.From(0).GetHashCode());

                MyStructInt.From(-1).GetHashCode().Should().Be(MyStructInt.From(-1).GetHashCode());
            }

            /// <summary>
            /// The same as record structs, GetHashCode only considers the underlying type and not the type itself.
            /// This is because it's unlikely you'd want to compare two structs of two different types (unless they're boxed).
            /// </summary>
            [Fact]
            public void DifferentStructsWithSameUnderlyingTypeAndValueHaveSameHashCode()
            {
                MyStructInt.From(0).GetHashCode().Should().Be(MyStructInt2.From(0).GetHashCode());

                MyStructInt.From(-1).GetHashCode().Should().Be(MyStructInt2.From(-1).GetHashCode());
            }
        }

        public class WithClasses
        {
            [Fact]
            public void SameStructsHaveSameHashCode()
            {
                MyClassInt.From(0).GetHashCode().Should().Be(MyClassInt.From(0).GetHashCode());

                MyClassInt.From(-1).GetHashCode().Should().Be(MyClassInt.From(-1).GetHashCode());
            }

            /// <summary>
            /// The same as record structs, GetHashCode only considers the underlying type and not the type itself.
            /// </summary>
            [Fact]
            public void DifferentClassesWithSameUnderlyingTypeAndValueHaveDifferentHashCode()
            {
                MyClassInt.From(0).GetHashCode().Should().NotBe(MyClassInt2.From(0).GetHashCode());

                MyClassInt.From(-1).GetHashCode().Should().NotBe(MyClassInt2.From(-1).GetHashCode());
            }

            [Fact]
            public void Storing_1()
            {
                var a1 = Age.From(18);
                var a2 = Age.From(50);

                var d = new Dictionary<Age, string>
                {
                    { a1, "hello1" },
                    { a2, "hello2" }
                };

                d.Count.Should().Be(2);

                d[a1].Should().Be("hello1");
                d[a2].Should().Be("hello2");
            }

            [Fact]
            public void Storing_2()
            {
                var a1 = Age.From(18);
                var a2 = Age.From(18);

                var d = new Dictionary<Age, string> { { a1, "hello1" } };

                d[a2] = "hello2";

                d.Count.Should().Be(1);

                d[a1].Should().Be("hello2");
            }
        }
    }
}
