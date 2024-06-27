#pragma warning disable VOG025

using Vogen.Tests.Types;

namespace ConsumerTests.RecordTests;

public class RecordClassCreationTests
{
    [Fact]
    public void Creation_Happy_Path_MyInt()
    {
        MyRecordClassInt vo1 = MyRecordClassInt.From(123);
        MyRecordClassInt vo2 = MyRecordClassInt.From(123);
    
        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Happy_Path_MyString()
    {
        MyRecordClassString vo1 = MyRecordClassString.From("123");
        MyRecordClassString vo2 = MyRecordClassString.From("123");

        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void Creation_Unhappy_Path_MyString()
    {
        Action action = () => MyRecordClassString.From(null!);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("Cannot create a value object with null.");
    }

    [Fact]
    public void Creation_Unhappy_Path_MyInt()
    {
        Action action = () => MyRecordClassInt.From(-1);
        
        action.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void Default_vo_throws_at_runtime()
    {
        MyRecordClassInt vo = (MyRecordClassInt) Activator.CreateInstance(Type.GetType("Vogen.Tests.Types.MyRecordClassInt")!)!;
        Func<int> action = () => vo.Value;

        action.Should().Throw<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
    }
}