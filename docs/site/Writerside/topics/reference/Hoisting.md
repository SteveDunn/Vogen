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
