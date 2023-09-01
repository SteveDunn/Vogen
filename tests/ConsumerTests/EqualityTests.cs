// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable RedundantCast
// ReSharper disable StringLiteralTypo
// ReSharper disable PossibleNullReferenceException
#pragma warning disable 252,253

using System.Collections;
using System.Diagnostics;
using Vogen.Tests.Types;

namespace ConsumerTests;

public class EqualityTests
{
    [Fact]
    public void equality_between_same_value_objects()
    {
        Age.From(18).Equals(Age.From(18)).Should().BeTrue();
        (Age.From(18) == Age.From(18)).Should().BeTrue();
            
        // uses the generated IEquatable<> 
        (Age.From(18) == 18).Should().BeTrue();
        (18 == Age.From(18)).Should().BeTrue();

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

    public class MyThing : IEquatable<MyThing>
    {
        public bool Equals(MyThing? other) => throw new NotImplementedException();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MyThing) obj);
        }

        public override int GetHashCode() => throw new NotImplementedException();
    }

    [Fact]
    public void equality_with_custom_IEqualityComparer()
    {
        Dictionary<Name, int> ages = new(10, new StringOrdinalIgnoreCaseComparer())
        {
            { Name.From("Steve"), 18 },
            { Name.From("Bob"), 19 },
            { Name.From("Alice"), 20 },
        };
        
        ages[Name.From("steve")].Should().Be(18);
        ages[Name.From("Steve")].Should().Be(18);
    }
    
    public class StringOrdinalIgnoreCaseComparer : IEqualityComparer<Name>
    {
        public bool Equals(Name? x, Name? y) => StringComparer.OrdinalIgnoreCase.Equals(x?.Value, y?.Value);

        public int GetHashCode(Name obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }

    [Fact]
    public void Nullness()
    {
        (Age.From(50) == null!).Should().BeFalse();
        Age.From(50).Equals(null!).Should().BeFalse();

        (Age.From(50) != null!).Should().BeTrue();
    }
}