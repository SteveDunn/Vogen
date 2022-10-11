namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial class MyInt
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

#if NET7_0_OR_GREATER
[ValueObject<int>]
public partial class MyIntGeneric
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}
#endif