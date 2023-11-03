# Instances

A type can have predefined 'instances'—examples include:

```c#
    [ValueObject(typeof(float))]
    [Instance("Freezing", 0.0f)]
    [Instance("Boiling", 100.0f)]
    [Instance("AbsoluteZero", -273.15f)]
    public readonly partial struct Centigrade
    {
        public static Validation Validate(float value) =>
            value >= AbsoluteZero.Value ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
    }
```

An `Instance` attribute is a name and value. The name can be any valid C# name, and the value can either be a value 
matching the underlying type, or a string that will be converted to the underlying type.

## Special attention for dates and times

For `DateTime` and `DateTimeOffset`, the instance value can be:

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

Even though you _can_ specify dates and times like this, it is probably better to specify them explicitly to avoid confusion