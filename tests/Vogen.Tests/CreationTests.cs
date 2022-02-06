using System;
using FluentAssertions;
using Vogen.Tests.Types;
using Xunit;

namespace Vogen.Tests;

public class CreationTests
{
    [Fact]
    public void Creation_Happy_Path_MyInt()
    {
        MyInt vo1 = MyInt.From(123);
        MyInt vo2 = MyInt.From(123);
    
        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Happy_Path_MyString()
    {
        MyString vo1 = MyString.From("123");
        MyString vo2 = MyString.From("123");

        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    // doesn't work - need to handle nested types
    // [Fact]
    // public void Creation_Happy_Path_EmbeddedType()
    // {
    //     var embedded = new TopLevelClass.AnotherClass.AndAnother.NestedType();
    //     embedded.From(123);
    // }

    [Fact]
    public void Creation_Unhappy_Path_MyString()
    {
        Action action = () => MyString.From(null!);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Cannot create a value object with null.");
    }

    [Fact]
    public void Creation_Unhappy_Path_MyInt()
    {
        Action action = () => MyInt.From(-1);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Default_vo_throws_at_runtime()
    {
        CustomerId[] ints = new CustomerId[10];
        Func<int> action = () => ints[0].Value;

        action.Should().Throw<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
    }
}