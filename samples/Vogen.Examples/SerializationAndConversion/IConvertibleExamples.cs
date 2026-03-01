using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vogen.Examples.Types;

namespace Vogen.Examples.SerializationAndConversion;

/// <summary>
/// Demonstrates the usefulness of IConvertible interface support in Vogen.
/// 
/// When a value object wraps a primitive type that implements IConvertible (like int, float, decimal, DateTime),
/// Vogen now generates the IConvertible interface on the value object as well. This enables seamless integration
/// with .NET APIs that expect IConvertible types.
/// </summary>
public class IConvertibleExamples : IScenario
{
    public Task Run()
    {
        DemonstrateConvertChangeType();
        DemonstrateConvertToSpecificType();
        DemonstrateCustomConversionLogic();
        DemonstrateCustomImplementations();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Example 1: Convert.ChangeType with IConvertible
    /// 
    /// The Convert.ChangeType method accepts IConvertible objects and can convert them to any target type.
    /// This is particularly useful in data mapping scenarios, ORMs, and serialization frameworks that need
    /// to dynamically convert values to different types based on runtime information.
    /// </summary>
    private void DemonstrateConvertChangeType()
    {
        // Create a value object wrapping a float (which implements IConvertible)
        var temperature = Celsius.From(25.5f);

        // Convert.ChangeType now works directly with the value object
        // This is essential for frameworks like reflection-based mappers, EF Core value converters, etc.
        object asInt = Convert.ChangeType(temperature, typeof(int));
        object asDecimal = Convert.ChangeType(temperature, typeof(decimal));
        object asString = Convert.ChangeType(temperature, typeof(string));

        Debug.Assert((int)asInt == 26);
        Debug.Assert((decimal)asDecimal == 25.5m);
        Debug.Assert((string)asString == "25.5");
    }

    /// <summary>
    /// Example 2: Converting to specific types via IConvertible methods
    /// 
    /// IConvertible provides explicit methods like ToInt32, ToDecimal, ToBoolean, etc. that return
    /// specific types. This is useful when you need type-safe conversions with explicit control over
    /// the target type, and when using APIs that expect IConvertible.
    /// </summary>
    private void DemonstrateConvertToSpecificType()
    {
        var temperature = Celsius.From(98.6f);

        // Convert to int using the IConvertible.ToInt32 method
        // This is useful when a library/API expects IConvertible and needs specific type conversions
        IConvertible convertible = temperature;
        int asInt = convertible.ToInt32(null);
        byte asByte = convertible.ToByte(null);

        Debug.Assert(asInt == 99);
        Debug.Assert(asByte == 99);
    }

    /// <summary>
    /// Example 3: Data mapping and dynamic conversion scenarios
    /// 
    /// IConvertible support is critical for frameworks that perform dynamic type conversions:
    /// - ORM tools that map database columns to object properties
    /// - API frameworks that convert query parameters to domain objects
    /// - Data transformation pipelines that process data with runtime type information
    /// 
    /// Without IConvertible, these frameworks would need special-case logic for value objects,
    /// but with it, they work seamlessly just like built-in types.
    /// </summary>
    private void DemonstrateCustomConversionLogic()
    {
        // Simulating a data mapping scenario where we have a target type determined at runtime
        var temperature = Celsius.From(32.0f);
        Type targetType = typeof(decimal);

        // This pattern is common in ORMs and data mapping libraries
        object convertedValue = Convert.ChangeType(temperature, targetType);

        Debug.Assert(convertedValue is decimal);
        Debug.Assert((decimal)convertedValue == 32.0m);

        Console.WriteLine($"Temperature {temperature} converted to {targetType.Name}: {convertedValue}");
    }

    /// <summary>
    /// Example 4: Custom IConvertible implementations
    /// 
    /// You can provide custom implementations for specific IConvertible methods.
    /// Vogen will respect your custom code and not override it, while still hoisting the other
    /// IConvertible methods. This is useful when you need special conversion logic for certain types.
    /// 
    /// The CustomRoundingFloat type demonstrates this: it provides a custom ToInt32 that rounds
    /// the float value instead of truncating it, while other IConvertible methods are automatically hoisted.
    /// </summary>
    private void DemonstrateCustomImplementations()
    {
        var value = CustomRoundingFloat.From(3.7f);

        // Uses the custom ToInt32 implementation (rounds to 4, not truncates to 3)
        IConvertible convertible = value;
        int rounded = convertible.ToInt32(null);

        Debug.Assert(rounded == 4);

        // Other IConvertible methods are still available and hoisted automatically
        byte asByte = convertible.ToByte(null);
        decimal asDecimal = convertible.ToDecimal(null);

        Debug.Assert(asByte == 4);
        Debug.Assert(asDecimal == 3.7m);

        Console.WriteLine($"CustomRoundingFloat {value} rounds to {rounded}, but converts to byte as {asByte} and decimal as {asDecimal}");
    }
}