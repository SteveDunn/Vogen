namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial class MyRecordClassInt
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}


[ValueObject(typeof(int))]
public partial class MyRecordStructInt
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}