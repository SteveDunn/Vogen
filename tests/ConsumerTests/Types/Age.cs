﻿namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public partial class Age
{
    private static Validation Validate(int value) => value >= 18 ? Validation.Ok : Validation.Invalid("Must be 18 or over");
}

[ValueObject<int>(primitiveEqualityGeneration: PrimitiveEqualityGeneration.Omit)]
public partial class AgeNoPrimitiveEquality
{
    private static Validation Validate(int value) => value >= 18 ? Validation.Ok : Validation.Invalid("Must be 18 or over");
}