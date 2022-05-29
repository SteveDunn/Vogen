using System;
using FluentAssertions;
using Vogen;
using Vogen.Tests.Types;
using Xunit;

namespace SmallTests.RecordTests;

public class RecordStructCreationTests
{
    [Fact]
    public void Creation_Happy_Path_MyInt()
    {
        MyRecordStructInt vo1 = MyRecordStructInt.From(123);
        MyRecordStructInt vo2 = MyRecordStructInt.From(123);
    
        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Happy_Path_MyString()
    {
        MyRecordStructString vo1 = MyRecordStructString.From("123");
        MyRecordStructString vo2 = MyRecordStructString.From("123");

        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Unhappy_Path_MyString()
    {
        Action action = () => MyRecordStructString.From(null!);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Cannot create a value object with null.");
    }

    [Fact]
    public void Creation_Unhappy_Path_MyInt()
    {
        Action action = () => MyRecordStructInt.From(-1);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Default_vo_throws_at_runtime()
    {
        MyRecordStructInt vo = (MyRecordStructInt) Activator.CreateInstance(typeof(MyRecordStructInt))!;
        Func<int> action = () => vo.Value;

        action.Should().Throw<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
    }
}