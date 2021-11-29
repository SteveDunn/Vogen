using Vogen.SharedTypes;

namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial class MinimalValidation
{
    private static Validation Validate(int value) => 
        value != 1 ? Validation.Ok : Validation.Invalid();
}