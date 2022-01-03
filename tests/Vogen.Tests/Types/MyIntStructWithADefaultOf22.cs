namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
[Instance(name: "Default", value: 22)]
[Instance(name: "Default2", value: 33)]
public partial struct MyIntStructWithADefaultOf22
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;
        
        return Validation.Invalid("must be greater than zero");
    }
}