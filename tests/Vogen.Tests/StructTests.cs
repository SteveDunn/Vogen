namespace Vogen.Tests.StructTests;

[ValueObject(typeof(int))]
public partial struct CustomerId
{
    private static Validation Validate(int value) =>
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject(typeof(string))]
public partial struct VendorName
{
    private static Validation Validate(string value) =>
        string.IsNullOrEmpty(value) ? Validation.Invalid("cannot be null or empty") : Validation.Ok;
}