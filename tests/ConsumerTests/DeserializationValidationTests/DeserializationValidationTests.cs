using System;
using System.ComponentModel;
using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.DeserializationValidationTests;

[ValueObject(typeof(int), Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowAnything)]
public partial class Vo_AllowAnything
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject(typeof(int), Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
[Instance("Unknown", -1)]
public partial class Vo_AllowValidAndKnownInstances
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject(typeof(int), Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
public partial class Vo_AllowValidAndKnownInstances_WithNormalizeMethod
{
    // the '-1' value was removed from the known instances (above),
    // so this type demonstrates how we can normalize the input to cater for it.
    private static int NormalizeInput(int input) => input == -1 ? 0 : input;

    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}
