# Configuration

Each value object can have its own *optional* configuration. Examples include:

* The underlying type
* Any 'conversions' (Dapper, System.Text.Json, Newtonsoft.Json, etc.) - see [Integrations](Integration.md) in the wiki for more information
* The type of the exception that is thrown when validation fails

Default values are inferred from global configuration. Global configuration is specified as an assembly level attribute. Here's an example:

```c#
[assembly: VogenDefaults(
    underlyingType: typeof(int), 
    conversions: Conversions.Default, 
    throws: typeof(ValueObjectValidationException))]
```

Most values are present in both `ValueObject` and `VogenDefaults`. The parameters for the  `ValueObject` attribute are:

* `underlyingType`—the type of the primitive that is being wrapped—defaults to `int`
* `conversions` - specified what conversion code is generated - defaults to `Conversions.Default` which generates type converters and a converter to handle serialization using System.Text.Json
* `throws` = specifies the type of exception thrown when validation fails—defaults to `ValueObjectValidationException`
* `customizations`—simple customization switches—defaults to `Customizations.None`,
* `deserializationStrictness` - specifies how strict deserialization is, e.g. should your `Validate` method be called, or should pre-defined instances that otherwise invalid be allowed - defaults to `DeserializationStrictness.AllowValidAndKnownInstances`
* `debuggerAttributes` - specifies the level that debug attributes are written as some IDEs don't support all of them, e.g., Rider - defaults to `DebuggerAttributeGeneration.Full` which generates `DebuggerDisplay` and a debugger proxy type for IDEs that support them
* `comparison`—species which comparison code is generated—defaults to `ComparisonGeneration.UseUnderlying` which hoists any `IComparable` implementations from the primitive
* `stringComparers` - specifies which string comparison code is generated—defaults to `StringComparersGeneration.Omit`, which doesn't generate anything related to string comparison 
* `toPrimitiveCasting` - specifies the type of casting from wrapper to primitive - defaults to `CastOperator.Explicit`,
* `fromPrimitiveCasting` - specifies the type of casting from primitive to wrapper - default to `CastOperator.Explicit`
* `parsableForStrings` - specifies what is generated for `IParsable` types for strings - defaults to `ParsableForStrings.GenerateMethodsAndInterface`
* `parsableForPrimitives` - specifies what is generated for `Parse` and `TryParse` methods - defaults to `ParsableForPrimitives.HoistMethodsAndInterfaces`
* `tryFromGeneration` - specifies what to write for `TryFrom` methods—defaults to `TryFromGeneration.GenerateBoolAndErrorOrMethods`
* `isInitializedMethodGeneration` - specifies whether to generate an `IsInitialized()` method - defaults to `IsInitializedMethodGeneration.Generate`
* `primitiveEqualityGeneration` - specified whether to generate primitive comparison operators.

The values that are specified only to global configuration are:

* `systemTextJsonConverterFactoryGeneration` - determines whether to write a factory for STJ converters which are useful as a replacement to Reflect in AOT/trimmed scenarios—defaults to `SystemTextJsonConverterFactoryGeneration.Generate`
* `staticAbstractsGeneration` - determines whether to write static abstract code - defaults to `StaticAbstractsGeneration.Omit`
* `openApiSchemaCustomizations` - determines what is generated to assist in OpenAPI scenarios, for instance, generate a schema filter for Swashbuckle, or generate an extension method with `MapType` calls - defaults to `OpenApiSchemaCustomizations.Omit`
* `explicitlySpecifyTypeInValueObject` - specifies whether individual value objects should explicitly define the primitive type that they wrap - 
  defaults to `false`
* `disableStackTraceRecordingInDebug` - disables stack trace recording; in Debug buids, a stack trace is recorded and is thrown in the exception when something is created in an uninitialized state, e.g. after deserialization

Several code analysis warnings exist for invalid configuration, including:

* when you specify an exception that does not derive from `System.Exception`
* when your exception does not have one public constructor that takes an int
* when the combination of conversions does not match an entry
