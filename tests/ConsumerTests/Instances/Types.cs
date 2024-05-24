namespace ConsumerTests.Instances;

[ValueObject]
public partial class IntWithNewedUpInstanceFields
{
    public static IntWithNewedUpInstanceFields Invalid = new(-1); 
    public static IntWithNewedUpInstanceFields Unspecified = new(-2); 
    
    // Error VOG027 : Type 'IntWithNewedUpInstanceFields' cannot be constructed as a field with 'new' as it should be public and static
    // public IntWithNewedUpInstanceFields NotStatic = new(-2); 
    
    // Error VOG027 : Type 'IntWithNewedUpInstanceFields' cannot be constructed as a field with 'new' as it should be public and static
    // internal static IntWithNewedUpInstanceFields NotPublic = new(-2); 
}

[ValueObject<float>]
public partial struct Centigrade
{
    public static readonly Centigrade WaterFreezingPoint = new(0.0f);
    public static readonly Centigrade WaterBoilingPoint = new(100.0f);
    public static readonly Centigrade AbsoluteZero = new(-273.15f);
}

[ValueObject<DateTime>]
public readonly partial struct DateTimeInstances
{
    public static readonly DateTimeInstances Date1 = new(DateTime.Parse("2022-12-13"));
    public static readonly DateTimeInstances Date2 = new(DateTime.Parse("2022-12-13T13:14:15Z"));
    public static readonly DateTimeInstances Date3 = new(new DateTime(638064864000000000L));
    public static readonly DateTimeInstances Date4 = new(new DateTime(2147483647));
}

[ValueObject<int>]
public readonly partial struct CustomerId
{
    // bypasses validation and normalization
    public static readonly CustomerId Unspecified = new(0);
    
    private static Validation Validate(int input) => input > 0 
        ? Validation.Ok 
        : Validation.Invalid("Customer IDs must be greater than 0.");    
}


// The above examples are the recommended approach to creating known instances.
// The examples below were a design that has proven problematic, but was once the 
// only way to create instances since `new` was disallowed, even in the value object itself.

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


