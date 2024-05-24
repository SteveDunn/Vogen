# Specify pre-set Instances

<card-summary>
How to specify pre-set instances with known values
</card-summary>

A type can have predefined 'instances'—examples include:

```C#
[ValueObject<int>]
public partial class CustomerId
{
    public static CustomerId Invalid = new(-1); 
    public static CustomerId Unspecified = new(-2); 

[ValueObject<float>]
public readonly partial struct Centigrade
{
    public static readonly Centigrade Freezing = new(0.0f);
    public static readonly Centigrade Boiling = new(100.0f);
    public static readonly Centigrade AbsoluteZero = new(-273.15f);
}
```

`new` is only allowed in the value object itself, and will allow you to create a value object that bypasses any validation you might have.
If you try to use new outside of the value object, then compilation will fail. 
If you know the value should be valid, and you have a `Validate` method, then use the `From` method instead, which will run your validation and normalization.

Previously in Vogen, you could only create instances via attributes because `new`, even inside the value object itself, was disallowed.

This design turned out to be troublesome. For instance, [decimals are not a constance in C#](https://codeblog.
jonskeet.uk/2014/08/22/when-is-a-constant-not-a-constant-when-its-a-decimal/), so this wasn't possible:

```C#
[ValueObject<decimal>]
[Instance("whatever", 123.45m)]
public partial class MyDecimal;
```

This results in `error CS0182: An attribute argument must be a constant expression, typeof expression or array creation expression of an attribute parameter type`

Other non-const types were also troublesome, e.g. `DateTime`. 
This couldn't be represented as a const, plus, representations could be either as ISO dates, or ticks. This resulted in declarations such as this:

```c#
[ValueObject(typeof(DateTime))]
[Instance(name: "iso8601_1", value: "2022-12-13")]             // uses `.Parse` using `RoundTripKind` - will be a local date
[Instance(name: "iso8601_2", value: "2022-12-13T13:14:15Z")]   // uses `.Parse` using `RoundTripKind`
[Instance(name: "ticks_as_long", value: 638064864000000000L)]  // uses ticks as UTC
[Instance(name: "ticks_as_int", value: 2147483647)]            // uses ticks as UTC
public readonly partial struct DateTimeInstances
{
}
```

It is much simpler to just write it as:

```C#
[ValueObject<DateTime>]
public readonly partial struct DateTimeInstances
{
    public static readonly DateTimeInstances Date1 = 
        new(DateTime.Parse("2022-12-13"));
    
    public static readonly DateTimeInstances Date2 = 
        new(DateTime.Parse("2022-12-13T13:14:15Z"));
    
    public static readonly DateTimeInstances Date3 = 
        new(new DateTime(638064864000000000L));
    
    public static readonly DateTimeInstances Date4 = 
        new(new DateTime(2147483647));
}
```

Please see [this post](https://github.com/SteveDunn/Vogen/issues/221) which described the need to relax the use
of `new` in the value object itself.