An example of using Vogen with System.Text.Json (STJ) source-generated serialization.

Reflection cannot be used in trimmed, and/or AOT compiled .NET applications.

Additionally, STJ converters, as produced by Vogen, aren't available
when STJ does its source generation.

The solution to this is for Vogen to source-generate a 'factory', which tells
STJ the respective converters for each value object generated.

A typical example is something like:

```c#
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    Converters =
    {
        new VogenTypesFactory()
    }
};

var ctx = new JsonSourceGenerationContext(options);

var json = JsonSerializer.Serialize(person, ctx.Person);
Person person2 = JsonSerializer.Deserialize(json, ctx.Person)!;
```

This sample produces a self-contained AOT binary for `win-x64`.

**Note**
If you _don't_ want the factory built, then this is configurable in the global config with:

`[assembly: VogenDefaults(
    systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit)]`