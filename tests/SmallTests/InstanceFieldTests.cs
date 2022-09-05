using Vogen;
using Vogen.Tests.Types;
using Xunit;
using FluentAssertions;

namespace SmallTests.InstanceFields;

public class InstanceFieldTests
{
    [Fact]
    public void Type_with_two_instance_fields()
    {
        _ = MyIntWithTwoInstanceOfInvalidAndUnspecified.From(100);

        MyIntWithTwoInstanceOfInvalidAndUnspecified.Invalid.Value.Should().Be(-1);
        MyIntWithTwoInstanceOfInvalidAndUnspecified.Unspecified.Value.Should().Be(-2);
    }

    [Fact]
    public void Struct_with_an_instance_field()
    {
        var sut = MyIntStructWithADefaultOf22.From(100);

        sut.Value.Should().Be(100);

        MyIntStructWithADefaultOf22.Default.Value.Should().Be(22);
        MyIntStructWithADefaultOf22.Default2.Value.Should().Be(33);
    }

    [Fact]
    public void Struct_with_an_instance_field_with_reserved_names()
    {
        var sut = MyIntStructWithReservedNames.From(100);

        sut.Value.Should().Be(100);

        MyIntStructWithReservedNames.@event.Value.Should().Be(22);
        MyIntStructWithReservedNames.@class.Value.Should().Be(33);
    }
}

[ValueObject(typeof(double))]
[UseCulture("fr-FR")]
[Instance(name: "Invalid", value: 123.45f)]
public partial class DoubleVo
{
}

[ValueObject(typeof(decimal))]
[UseCulture("fr-FR")]
[Instance(name: "Invalid", value: 123.45)]
public partial class DecimalVo
{
}

[ValueObject(typeof(int))]
[Instance(name: "Invalid", value: -1)]
[Instance(name: "Unspecified", value: -2)]
public partial class MyIntWithTwoInstanceOfInvalidAndUnspecified
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;
        
        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(typeof(int))]
[Instance(name: "Invalid", value: -1)]
[Instance(name: "Unspecified", value: -2)]
public partial class MyStructVoIntWithTwoInstanceOfInvalidAndUnspecified
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;
        
        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(typeof(int))]
[Instance(name: "Invalid", value: -1)]
[Instance(name: "Unspecified", value: -2)]
public partial class MyRecordClassVoIntWithTwoInstanceOfInvalidAndUnspecified
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;
        
        return Validation.Invalid("must be greater than zero");
    }
}