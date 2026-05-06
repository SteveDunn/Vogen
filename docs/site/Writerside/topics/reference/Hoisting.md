# Hoisting

Vogen 'hoists' functionality from the underlying primitive. For instance, if the underlying type implements `IComparable<>`, then the code that Vogen generates will also implement `IComparable<>` with the generic argument being the type of wrapper.

Here is what is hoisted:

## Parsing        

Any method named `Parse` or `TryParse` from the underlying primitive is hoisted.
Also, the `IParsable`
family of interfaces (including `ISpanParsable` and `IUtf8SpanParsable`)
that are **implemented publicly** by the primitive are hoisted.

Please see the [Parsing](Parsing.md) documentation for more information.

## IComparable

If the underlying primitive implements this, and the configuration for `ComparisonGeneration` is `UseUnderlying`, then the generated wrapper will implement `IComparable<>` and `IComparable`, with the generic argument being the type of wrapped primitive.
The method generated is `public int CompareTo([primitive] other)...`

## INumber / INumberBase

If the underlying primitive implements `INumber<T>` or `INumberBase<T>` (.NET 7+), Vogen can generate the corresponding interface implementation on the wrapper, including all arithmetic operators, comparison operators, and numeric helpers.

This is opt-in via `NumericsGeneration.Generate` on the `[ValueObject]` attribute:

```C#
[ValueObject<double>(numericsGeneration: NumericsGeneration.Generate)]
public partial struct Distance { }
```

Vogen automatically selects the richest interface the underlying type supports:
- `INumber<T>` for signed and unsigned integer types and floating-point types (e.g. `int`, `double`, `uint`)
- `INumberBase<T>` for types that implement it but not `INumber<T>` (e.g. `System.Numerics.Complex`)

Requires C# 11 or later. If the underlying type does not implement `INumberBase<T>` at all, a [VOG037](Analyzer-Rules.md) warning is emitted and nothing is generated.

> **Note:** Types whose `IParsable<T>` implementation is explicit-only (e.g. `char`) will not have numeric interfaces generated because Vogen cannot hoist `Parse`/`TryParse`.

## IConvertible

If the underlying primitive implements `IConvertible`, then the generated wrapper will also implement `IConvertible`. This enables seamless integration with .NET APIs and frameworks that expect `IConvertible` types, such as:

- `Convert.ChangeType()` for dynamic type conversion
- Data mapping frameworks and ORMs
- Reflection-based serialization utilities

The methods generated delegate to the corresponding methods on the underlying primitive value. This includes methods like `ToInt32()`, `ToDecimal()`, `ToBoolean()`, etc.

**Note:** You can provide custom implementations for specific `IConvertible` methods if needed. Vogen will respect your custom code and only hoist the remaining methods.

For detailed information, see [IConvertible](IConvertible.md) and [Conversion Mechanisms](conversion-mechanisms.md).
