using Vogen.SharedTypes;

namespace Vogen.Tests.Types;

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