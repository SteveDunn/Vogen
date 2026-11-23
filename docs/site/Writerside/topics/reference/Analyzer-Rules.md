# Analyzer Rules

Vogen emits the following diagnostics. Errors (VOGxxx) prevent code generation; warnings advise on configuration that has no effect.

| ID | Severity | Description |
|----|----------|-------------|
| VOG001 | Error | Types cannot be nested |
| VOG002 | Error | Underlying type must not be the same as the value object |
| VOG003 | Error | Underlying type cannot be a collection |
| VOG004 | Error | Validation method must return `Validation` |
| VOG005 | Error | Validate method must be static |
| VOG006 | Error | Instance method cannot have a null argument name |
| VOG007 | Error | Instance method cannot have a null argument value |
| VOG008 | Error | Cannot have user-defined constructors |
| VOG009 | Error | Do not use `default` to create a value object |
| VOG010 | Error | Do not use `new` to create a value object |
| VOG011 | Error | Invalid `Conversions` combination |
| VOG012 | Error | Custom exception must derive from `System.Exception` |
| VOG013 | Error | Custom exception must have a valid constructor |
| VOG014 | Error | NormalizeInput method must be static |
| VOG015 | Error | NormalizeInput method must return the same underlying type |
| VOG016 | Error | NormalizeInput method must take one parameter of the underlying type |
| VOG017 | Error | Types cannot be abstract |
| VOG019 | Error | Invalid `Customizations` combination |
| VOG020 | Error | `ToString` override on a record should be sealed |
| VOG021 | Error | Value object type should be partial |
| VOG022 | Error | Invalid `DeserializationStrictness` combination |
| VOG023 | Error | Instance value cannot be converted to the underlying type |
| VOG024 | Error | Duplicate value object types found |
| VOG025 | Error | Do not use reflection to create a value object |
| VOG026 | Error | Do not derive from Vogen attributes |
| VOG027 | Error | Incorrect use of an instance field |
| VOG028 | Error | Incorrect use of a NormalizeInput method |
| VOG029 | Error | Type must explicitly specify the underlying type in the `ValueObject` attribute |
| VOG031 | Error | Types referenced in a conversion marker must be value objects |
| VOG032 | Error | Do not throw from user validation or normalization code |
| VOG033 | Warning | Prefer `readonly struct` instead of `struct` |
| VOG034 | Warning | Do not compare with primitives in EF Core queries |
| VOG035 | Error | Value object referenced in a conversion marker must explicitly specify its primitive type |
| VOG036 | Error | Both implicit and explicit casts specified |
| VOG037 | Warning | `NumericsGeneration.Generate` has no effect because the underlying type does not implement `INumberBase<T>` |
| VOG041 | Error | NormalizeParsed method must be static |
| VOG042 | Error | NormalizeParsed method must return a string |
| VOG043 | Error | NormalizeParsed method must take one string parameter |
