namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial class Score
{
    private static Validation Validate(int value) => value >= 0 ? Validation.Ok : Validation.Invalid("Score must be zero or more");
}