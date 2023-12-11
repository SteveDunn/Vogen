# Configuration

<note>
This topic is incomplete and is currently being improved.
</note>


Each Value Object can have its own *optional* configuration. Configuration includes:

* The underlying type
* Any 'conversions' (Dapper, System.Text.Json, Newtonsoft.Json, etc.) - see [the Integrations page](https://github.com/SteveDunn/Vogen/wiki/Integration) in the wiki for more information
* The type of the exception that is thrown when validation fails

If any of those above are not specified, then global configuration is inferred. It looks like this:

```c#
[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.Default, throws: typeof(ValueObjectValidationException))]
```

Those again are optional. If they're not specified, then they are defaulted to:

* Underlying type = `typeof(int)`
* Conversions = `Conversions.Default` (`TypeConverter` and `System.Text.Json`)
* Validation exception type = `typeof(ValueObjectValidationException)`

There are several code analysis warnings for invalid configuration, including:

* when you specify an exception that does not derive from `System.Exception`
* when your exception does not have one public constructor that takes an int
* when the combination of conversions does not match an entry
