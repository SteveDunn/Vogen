# IConvertible

## Overview

When the underlying primitive type implements the `System.IConvertible` interface (such as `int`, `float`, `decimal`, `DateTime`, etc.), Vogen automatically generates the `IConvertible` interface on the value object as well.

This enables seamless integration with .NET APIs that work with `IConvertible` types, such as:
- `Convert.ChangeType()` for dynamic type conversion
- Data mapping frameworks and ORMs that expect `IConvertible`
- Reflection-based serialization and conversion utilities

## Why Vogen Generates IConvertible

The `IConvertible` interface provides a standardized way for types to participate in runtime type conversion. Many .NET frameworks rely on this interface to perform dynamic conversions without knowing the specific type at compile time.

**Example scenarios where IConvertible is essential:**

1. **Dynamic ORM mapping** - Entity Framework, Dapper, and other ORMs may use `Convert.ChangeType()` when mapping database columns to object properties with types determined at runtime.

2. **Framework reflection** - ASP.NET Core parameter binding, data binding in WPF, and other reflection-based frameworks may rely on `IConvertible` for flexible type conversion.

3. **Generic conversion utilities** - Libraries that provide generic data transformation pipelines often depend on `IConvertible` to convert values to arbitrary target types.

By hoisting `IConvertible` from the underlying primitive to the value object, Vogen ensures your value objects work seamlessly with these frameworks without requiring special-case logic.

## How It Works

Vogen hoists the `IConvertible` methods from the underlying primitive to the value object. These methods include:

- `ToBoolean(IFormatProvider)`
- `ToByte(IFormatProvider)`
- `ToChar(IFormatProvider)`
- `ToDateTime(IFormatProvider)`
- `ToDecimal(IFormatProvider)`
- `ToDouble(IFormatProvider)`
- `ToInt16(IFormatProvider)`
- `ToInt32(IFormatProvider)`
- `ToInt64(IFormatProvider)`
- `ToSByte(IFormatProvider)`
- `ToSingle(IFormatProvider)`
- `ToString(IFormatProvider)`
- `ToType(Type, IFormatProvider)`
- `GetTypeCode()`

**Method behavior:** Each hoisted method delegates to the corresponding method on the underlying primitive value, allowing the conversion logic to be inherited from the primitive while respecting the value object's initialization state.

## Example Usage

### Basic Conversion with Convert.ChangeType

```c#
[ValueObject<int>]
public partial struct UserId { }

var userId = UserId.From(42);

// IConvertible enables Convert.ChangeType to work directly with the value object
object asString = Convert.ChangeType(userId, typeof(string));  // "42"
object asDecimal = Convert.ChangeType(userId, typeof(decimal)); // 42m
```

### Using IConvertible Methods Directly

```c#
[ValueObject<float>]
public readonly partial struct Temperature { }

var temp = Temperature.From(25.5f);

IConvertible convertible = temp;
int asInt = convertible.ToInt32(null);      // 26 (rounded)
byte asByte = convertible.ToByte(null);     // 26
string asString = convertible.ToString(null); // "25.5"
```

### Dynamic Type Conversion in Data Mapping

```c#
[ValueObject<string>]
public partial class ProductCode { }

// In a generic data mapping utility
public object ConvertValue(IConvertible value, Type targetType)
{
    return Convert.ChangeType(value, targetType);
}

var code = ProductCode.From("ABC-123");
object asInt = ConvertValue(code, typeof(int)); // Framework handles conversion
```

## Custom Implementations

You can provide custom implementations for specific `IConvertible` methods if the default hoisted behavior doesn't meet your needs. Vogen will respect your custom implementations and not override them.

```c#
[ValueObject<float>]
public partial struct CustomRoundingFloat
{
    /// Custom ToInt32 that rounds instead of truncating
    public int ToInt32(IFormatProvider? provider)
    {
        return IsInitialized() ? (int)Math.Round(Value) : 0;
    }
    // Other IConvertible methods are automatically hoisted
}

var value = CustomRoundingFloat.From(3.7f);
IConvertible convertible = value;
int rounded = convertible.ToInt32(null); // 4 (rounded, not 3)
```

## Initialization State

Hoisted `IConvertible` methods check if the value object is initialized before delegating to the underlying primitive. If uninitialized, they return the default value for the target type.

```c#
[ValueObject<int>]
public partial struct Count { }

var uninitialized = default(Count);

IConvertible convertible = uninitialized;
int asInt = convertible.ToInt32(null); // Returns 0 (default)
```

## When to Use IConvertible vs. Other Conversion Mechanisms

While `IConvertible` is powerful, Vogen provides several conversion mechanisms suited to different scenarios:

| Mechanism | Use Case | Type Safety | Performance |
|-----------|----------|-------------|-------------|
| **IConvertible** | Dynamic runtime conversion, framework integration | Medium (runtime dispatch) | Lower (reflection-based) |
| **Explicit Casting** | Type-safe domain logic conversions | High (compile-time) | High (direct code) |
| **TypeConverter** | Framework parameter binding (ASP.NET Core, WPF) | Medium (string-based) | Medium |
| **Serialization** (JSON, BSON) | API/database integration | High (schema-driven) | Depends |

For more detailed guidance, see [Conversion Mechanisms](conversion-mechanisms.md).

## See Also

- [Hoisting](Hoisting.md) - General hoisting strategy in Vogen
- [Integration](Integration.md) - Converting and serializing value objects
- [Casting Operators](Casting.md) - Alternative conversion via explicit/implicit casts
