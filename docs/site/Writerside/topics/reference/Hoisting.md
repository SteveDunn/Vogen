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

## IConvertible

If the underlying primitive implements `IConvertible`, then the generated wrapper will also implement `IConvertible`. This enables seamless integration with .NET APIs and frameworks that expect `IConvertible` types, such as:

- `Convert.ChangeType()` for dynamic type conversion
- Data mapping frameworks and ORMs
- Reflection-based serialization utilities

The methods generated delegate to the corresponding methods on the underlying primitive value. This includes methods like `ToInt32()`, `ToDecimal()`, `ToBoolean()`, etc.

**Note:** You can provide custom implementations for specific `IConvertible` methods if needed. Vogen will respect your custom code and only hoist the remaining methods.

For detailed information, see [IConvertible](IConvertible.md) and [Conversion Mechanisms](../how-to/conversion-mechanisms.md).
