using System;
using Vogen.SharedTypes;

namespace Vogen.Tests.Types;

[ValueObject(typeof(string))]
public partial class Dave
{
    private static Validation Validate(string value) => value.StartsWith("dave ", StringComparison.OrdinalIgnoreCase) ||
                                                        value.StartsWith("david ", StringComparison.OrdinalIgnoreCase)
        ? Validation.Ok
        : Validation.Invalid("must be a dave or david");
}