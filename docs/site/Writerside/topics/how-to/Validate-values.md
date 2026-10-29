# Validate values

<card-summary>
Validate the value wrapped by a value object, control what happens when validation fails, and choose whether validation runs during deserialization.
</card-summary>

Vogen can validate the underlying value whenever an instance is created. Add a method named `Validate` to the value object, and Vogen generates code that calls it from `From` and `TryFrom`, and — if configured — from deserialization code.

The [validation tutorial](ValidationTutorial.md) shows the basics. This article describes the rules for the method, what happens when validation fails, and how validation interacts with normalization and deserialization.

## Add a Validate method

Vogen looks for a method named `Validate` that:

* is `static` — the analyzer reports [VOG005](Analyzer-Rules.md) if it isn't;
* returns `Vogen.Validation` — the analyzer reports [VOG004](Analyzer-Rules.md) if it doesn't;
* has a single parameter, which receives the underlying value.

The method is usually `private`, because if code holds an instance of the value object, the value is already known to be valid; there's no need for callers to validate it themselves.

Return `Validation.Ok` when the value is valid, or `Validation.Invalid` with a reason when it isn't:

```c#
[ValueObject<int>]
public partial struct CustomerId
{
    private static Validation Validate(int input) =>
        input > 0 ? Validation.Ok : Validation.Invalid("Customer IDs must be greater than 0.");
}
```

`CustomerId.From(5)` returns an instance. `CustomerId.From(0)` throws an exception whose message is the reason given to `Validation.Invalid`:

```
Vogen.ValueObjectValidationException: Customer IDs must be greater than 0.
```

The default exception type is `Vogen.ValueObjectValidationException`; see [Use your own exception type](#use-your-own-exception-type) to change it.

## Describe the failure

The message passed to `Validation.Invalid` becomes the exception message. If you don't pass a message, the exception message is `[none provided]`.

`Validation.WithData` copies extra data to the `Data` dictionary of the thrown exception, which is useful for logging or telemetry:

```c#
private static Validation Validate(int input) =>
    input > 0
        ? Validation.Ok
        : Validation.Invalid("must be positive").WithData("input", input);
```

The data is available on the exception, for example `exception.Data["input"]`.

<note>
Don't throw from `Validate` (or from `NormalizeInput`). Return `Validation.Invalid` instead and let Vogen create and throw the exception; throwing from these methods is flagged by the analyzer rule [VOG032](Analyzer-Rules.md), because the exception can surface in surprising places, such as implicit conversions.
</note>

## Use your own exception type

Specify the exception type with the `throws` parameter of the `ValueObject` attribute, or globally with `VogenDefaults`. The type must:

* derive from `System.Exception` — the analyzer reports [VOG012](Analyzer-Rules.md) otherwise;
* have at least one `public` constructor with a single `string` parameter — the analyzer reports [VOG013](Analyzer-Rules.md) otherwise.

```c#
public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}

[ValueObject<int>(throws: typeof(MyValidationException))]
public partial struct OrderId
{
    private static Validation Validate(int input) =>
        input > 0 ? Validation.Ok : Validation.Invalid("must be positive");
}
```

`OrderId.From(0)` now throws `MyValidationException` instead of `ValueObjectValidationException`.

## Normalization runs before validation

If the type has a `NormalizeInput` method, Vogen normalizes the value before it calls `Validate`, so `Validate` always sees the normalized value. The example below trims the input and then rejects empty strings:

```c#
[ValueObject<string>]
public partial class ScrapedString
{
    private static Validation Validate(string value) =>
        value.Length == 0 ? Validation.Invalid("Can't be empty") : Validation.Ok;

    private static string NormalizeInput(string input) => input.Trim();
}
```

`ScrapedString.From("   ")` trims the input to an empty string and then fails validation with `Can't be empty`. See [Normalize input values](NormalizationHowTo.md) for the rules for `NormalizeInput`.

## Validate without throwing: TryFrom

By default, Vogen also generates `TryFrom` methods that report validation failures without throwing. The generated overloads are controlled by the `tryFromGeneration` parameter — see [Configuration](Configuration.md).

The boolean overload returns `false` when validation fails:

```c#
CustomerId.TryFrom(0, out CustomerId invalid);   // false
CustomerId.TryFrom(5, out CustomerId valid);     // true, valid.Value is 5
```

The `ValueObjectOrError<T>` overload returns either the value object or the failed `Validation`:

```c#
ValueObjectOrError<CustomerId> result = CustomerId.TryFrom(0);

result.IsSuccess;              // false
result.Error.ErrorMessage;     // Customer IDs must be greater than 0.
result.ValueObject;            // throws InvalidOperationException
```

## Choose when validation runs during deserialization

Deserializers construct value objects from the stored primitive value rather than calling `From`, so whether `Validate` runs is controlled by the `deserializationStrictness` parameter of the `ValueObject` attribute. The default is `AllowValidAndKnownInstances`, which runs `Validate` and also accepts values declared as predefined instances.

| Setting | Effect |
|---|---|
| `AllowAnything` | Accepts any value; `Validate` isn't called. |
| `AllowKnownInstances` | Accepts values declared as predefined instances; other values are accepted without calling `Validate`. |
| `RunMyValidationMethod` | Calls `Validate` for deserialized values. |
| `AllowValidAndKnownInstances` | Both of the above — the default. |
| `DisallowNulls` | `System.Text.Json` only: reject `null` for reference-type value objects. Combine with another setting as needed. |

Because the values are flags, you can combine them, for example `DeserializationStrictness.AllowKnownInstances | DeserializationStrictness.RunMyValidationMethod`.

With the default strictness, deserializing an invalid value fails. `System.Text.Json` wraps the validation exception in a `JsonException`:

```c#
JsonSerializer.Deserialize<CustomerId>("0");
// System.Text.Json.JsonException: The JSON value could not be converted to CustomerId.
//   Inner exception: Vogen.ValueObjectValidationException: Customer IDs must be greater than 0.

JsonSerializer.Deserialize<CustomerId>("5").Value; // 5
```

With `AllowAnything`, the value is accepted without validation:

```c#
[ValueObject<int>(deserializationStrictness: DeserializationStrictness.AllowAnything)]
public partial struct LooseId
{
    private static Validation Validate(int input) =>
        input > 0 ? Validation.Ok : Validation.Invalid("must be positive");
}

JsonSerializer.Deserialize<LooseId>("0").Value; // 0
```

## Validation and predefined instances

Values declared with the `Instance` attribute (see [Specifying pre-set values](Specifying-pre-set-values.md)) are trusted:

* they aren't validated at compile time, so an instance can hold a value that `Validate` would reject;
* `From` still validates a value, even if it matches a predefined instance, so `CustomerId.From(0)` fails even when an instance with the value `0` exists;
* deserialization returns the matching predefined instance without calling `Validate` when `AllowKnownInstances` is in effect, which is the default.

```c#
[ValueObject<int>]
[Instance("Unknown", 0)]
public partial struct KnownStateId
{
    private static Validation Validate(int input) =>
        input > 0 ? Validation.Ok : Validation.Invalid("must be positive");
}

KnownStateId.From(0);                                     // throws — From always validates
JsonSerializer.Deserialize<KnownStateId>("0").Value;      // 0 — the predefined instance is accepted
JsonSerializer.Deserialize<KnownStateId>("-1");           // throws — not a known instance
```

## Related

* [Validation tutorial](ValidationTutorial.md)
* [Configuration](Configuration.md) — all parameters of the `ValueObject` attribute
* [Analyzer rules](Analyzer-Rules.md) — the `VOG` diagnostics mentioned above
* [Normalize input values](NormalizationHowTo.md)
* [Parsing](Parsing.md) — `Parse` and `TryParse` also take `Validate` into account
