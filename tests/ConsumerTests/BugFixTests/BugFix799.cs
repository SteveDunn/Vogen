namespace ConsumerTests.BugFixTests.BugFix799;

[ValueObject<string>]
public partial class Class;

[ValueObject<string>]
public partial struct Struct;

[ValueObject<string>]
public partial record class RecordClass;

[ValueObject<string>]
public partial record struct RecordStruct;

public class Tests
{
    [Fact]
    public void Class_should_throw_when_given_null()
    {
        Action a1 = () => Class.From(null!);
        a1.Should().ThrowExactly<ValueObjectValidationException>();
    }

    [Fact]
    public void Struct_should_throw_when_given_null()
    {
        Action a1 = () => Struct.From(null!);
        a1.Should().ThrowExactly<ValueObjectValidationException>();
    }

    [Fact]
    public void Record_class_should_throw_when_given_null()
    {
        Action a1 = () => RecordClass.From(null!);
        a1.Should().ThrowExactly<ValueObjectValidationException>();
    }

    [Fact]
    public void Record_struct_should_throw_when_given_null()
    {
        Action a1 = () => RecordStruct.From(null!);
        a1.Should().ThrowExactly<ValueObjectValidationException>();
    }
}