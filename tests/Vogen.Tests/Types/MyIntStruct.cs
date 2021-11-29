using Vogen.SharedTypes;

namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial struct MyIntStruct
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;
        
        return Validation.Invalid("must be greater than zero");
    }
}