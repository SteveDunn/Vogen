namespace Vogen.Tests.ValidationTests;

[ValueObject]
public partial struct VoStruct
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(typeof(int))]
public partial class MyVo_validate_with_camelCase_method_name
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(typeof(int))]
public partial class MyVo_validate_with_synonymous_input_parameter
{
    // the input type is synonymous the same as the underlying type
    private static Validation validate(Int32 value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}


[ValueObject(typeof(int))]
public partial class MyVo_validate_with_PascalCase_method_name
{
    private static Validation Validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject(throws: typeof(InvalidAmountException))]
public partial class MyVo_throws_custom_exception
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

// Does not compile:
// VOG012 : String must derive from System.Exception
// VOG013 : String must have at least 1 public constructor with 1 parameter of type System.String
// [ValueObject(throws: typeof(string))]
// public partial class MyVo_throws_non_exception
// {
//     private static Validation Validate(int value) => 
//         value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
// }

public class InvalidAmountException : Exception
{
    public InvalidAmountException()
    {
    }

    public InvalidAmountException(string? message) : base(message)
    {
    }
}
