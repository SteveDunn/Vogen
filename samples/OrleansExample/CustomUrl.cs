using Vogen;

namespace OrleansExample;

[ValueObject<string>(Conversions.Default | Conversions.Orleans)]
public partial class CustomUrl
{
    private static Validation Validate(string value)
    {
        // Just for example’s sake, a real Url validator should be more complex then this.
        return value.StartsWith("http")
            ? Validation.Ok
            : Validation.Invalid("A url should start with http");
    }
}