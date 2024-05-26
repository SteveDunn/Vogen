namespace Vogen.Tests.Types;

[ValueObject<float>]
public readonly partial struct CustomerId {
    private static Validation Validate(float input)
    {
        bool isValid = true; // todo: your validation
        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
    }
    private static float NormalizeInput(string input)
    {
        // todo: normalize (sanitize) your input;
        return 1f;
    }
}