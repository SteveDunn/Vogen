// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo
// ReSharper disable PossibleNullReferenceException
#pragma warning disable 252,253

using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using Vogen.SharedTypes;
using Vogen.Tests.Types;
using Xunit;

namespace Vogen.Tests
{
    public class ValueObjectTests
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
        public void Validation()
        {
            Func<Age> act = () => Age.From(12);
            act.Should().ThrowExactly<ValueObjectValidationException>();

            Func<EightiesDate> act2 = () => EightiesDate.From(new DateTime(1990, 1,1));
            act2.Should().ThrowExactly<ValueObjectValidationException>();

            Func<EightiesDate> act3 = () => EightiesDate.From(new DateTime(1985, 6,10));
            act3.Should().NotThrow();

            string[] validDaves = new[] { "dave grohl", "david beckham", "david bowie" };
            foreach (var name in validDaves)
            {
                Func<Dave> act4 = () => Dave.From(name);
                act4.Should().NotThrow();
            }

            string[] invalidDaves = new[] { "dafid jones", "fred flintstone", "davidoff cool water" };
            foreach (var name in invalidDaves)
            {
                Func<Dave> act5 = () => Dave.From(name);
                act5.Should().ThrowExactly<ValueObjectValidationException>();
            }

            Func<MinimalValidation> act6 = () => MinimalValidation.From(1);
            act6.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("[none provided]");
        }

        [Fact]
        public void No_validation()
        {
            Func<Anything> act = () => Anything.From(int.MaxValue);
            act.Should().NotThrow();
        }

        [Fact]
        public void Nullness()
        {
            (Age.From(50) == null!).Should().BeFalse();
            Age.From(50).Equals(null!).Should().BeFalse();

            (Age.From(50) != null!).Should().BeTrue();
        }

        [Fact]
        public void Hashing()
        {
            (Age.From(18).GetHashCode() == Age.From(18).GetHashCode()).Should().BeTrue();
            (Age.From(18).GetHashCode() == Age.From(19).GetHashCode()).Should().BeFalse();
            (Age.From(18).GetHashCode() == Score.From(1).GetHashCode()).Should().BeFalse();
            (Age.From(18).GetHashCode() == Score.From(18).GetHashCode()).Should().BeFalse();
        }
        
        [Fact]
        public void To_string()
        {
            Age.From(18).ToString().Should().Be("18");
            Age.From(100).ToString().Should().Be("100");
            Age.From(1_000).ToString().Should().Be("1000");
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