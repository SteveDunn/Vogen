using System;

namespace Vogen.Tests.Types;

[ValueObject(typeof(DateTime))]
public partial class EightiesDate
{
    private static Validation Validate(DateTime value) => value.Year is >= 1980 and <= 1989
        ? Validation.Ok
        : Validation.Invalid("Must be a date in the 80's");
}