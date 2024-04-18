# Parsing

Vogen 'hoists' (copies up to the wrapper) certain functionality from the underlying primitive. For instance, any method named `Parse` or `TryParse` from the underlying primitive is hoisted.

The `IParsable` family of interfaces (including `ISpanParsable` and `IUtf8SpanParsable`) that are **implemented 
publicly** by the primitive are hoisted to the wrapper and the generic parameter is changed to that of the wrapper. The methods that are generated delegate back to the underlying implementation of primitive.
   
Some primitive types, such as `bool`, explicitly implement `ISpanParsable<bool>` **privately**, so the _interface_ is not 
hoisted to wrapper, but the _non-explicit_ methods *are* hoisted.

`TryParse` calls the underlying's `TryParse`. It then sees if the parsed value passes the `Validate` method. If it 
doesn't, then it will return `false`, and the `out` will have a default instance; for classes, a `null` 
value, or, for structs, a `default` value object that will throw if you try to access its value.

`string`s are a special case. It is useful to have a Parse/TryParse methods on these, for instance, when value objects represent parameters in ASP.NET Core endpoints.

If `IParsable<>` is not available, e.g. in versions before .NET 7, then the interfaces are not generated for the wrapper. For `strings`, the `Parse` and `TryParse` methods are still generated though.

<note>
Beginning with V4.0, the behaviour of `TryParse` has changed. In previous versions, `TryParse` would throw a `ValueObjectValidationException`. 
With hindsight, this doesn't make sense and doesn't fit in with the idiomatic use of the `TryParse` pattern.
Also changed is that by default, a value object wrapping a string will automatically generate `IParsable`, `ISpanParsable`, and `IUtf8SpanParsable`.
</note>

