// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo
// ReSharper disable PossibleNullReferenceException
#pragma warning disable 252,253

using System.Diagnostics;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit;

namespace ConsumerTests
{
    public class EqualityTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            Age.From(18).Equals(Age.From(18)).Should().BeTrue();
            (Age.From(18) == Age.From(18)).Should().BeTrue();

            (Age.From(18) != Age.From(19)).Should().BeTrue();
            (Age.From(18) == Age.From(19)).Should().BeFalse();

            Age.From(18).Equals(Age.From(18)).Should().BeTrue();
            (Age.From(18) == Age.From(18)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            Age.From(18).Equals(Score.From(18)).Should().BeFalse();
            (Age.From(18) == (object)Score.From(18)).Should().BeFalse();
        }

        [Fact]
        public void equality_with_primitives()
        {
            Age.From(18).Equals(-1).Should().BeFalse();
            (Age.From(18) == 18).Should().BeTrue();
            (18 == Age.From(18)).Should().BeTrue();
            Age.From(18).Equals(18).Should().BeTrue();

            (Age.From(18) != Age.From(19)).Should().BeTrue();
            (Age.From(18) != 2).Should().BeTrue();
            (Age.From(18) == 2).Should().BeFalse();
            Age.From(18).Equals(Age.From(19)).Should().BeFalse();

            Age.From(18).Equals(new StackFrame()).Should().BeFalse();
            
            Age.From(18).Equals(Score.From(1)).Should().BeFalse();
        }

        [Fact]
        public void reference_equality()
        {
            var age1 = Age.From(18);
            var age2 = Age.From(18);

            age1.Equals(age1).Should().BeTrue();

            object.ReferenceEquals(age1, age2).Should().BeFalse();
        }

        [Fact]
        public void equality_with_object()
        {
            var age = Age.From(18);
            age.Equals((object)age).Should().BeTrue();
            
            age.Equals((object)"???").Should().BeFalse();

            (age == (object) "??").Should().BeFalse();
            (age != (object) "??").Should().BeTrue();
        }

        [Fact]
        public void Nullness()
        {
            (Age.From(50) == null!).Should().BeFalse();
            Age.From(50).Equals(null!).Should().BeFalse();

            (Age.From(50) != null!).Should().BeTrue();
        }
    }
}