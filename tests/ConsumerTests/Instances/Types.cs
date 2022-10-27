using Vogen;

namespace ConsumerTests.Instances;

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

[ValueObject(typeof(decimal))]
[Instance(name: "Invalid", value: "-1.23")]
[Instance(name: "Unspecified", value: "-2.34")]
public partial class MyDecimalWithTwoInstanceOfInvalidAndUnspecified
{
    private static Validation Validate(decimal value)
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