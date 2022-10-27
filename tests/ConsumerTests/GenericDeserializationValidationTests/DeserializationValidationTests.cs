using System;
using System.ComponentModel;
using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.GenericDeserializationValidationTests;

#if NET7_0_OR_GREATER
[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowAnything)]
public partial class Vo_AllowAnything
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
[Instance("Unknown", -1)]
public partial class Vo_AllowValidAndKnownInstances
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
public partial class Vo_AllowValidAndKnownInstances_WithNormalizeMethod
{
    // the '-1' value was removed from the known instances (above),
    // so this type demonstrates how we can normalize the input to cater for it.
    private static int NormalizeInput(int input) => input == -1 ? 0 : input;

    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowAnything)]
public partial class VoGeneric_AllowAnything
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
[Instance("Unknown", -1)]
public partial class VoGeneric_AllowValidAndKnownInstances
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

[ValueObject<int>(Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance("Invalid", 0)]
public partial class VoGeneric_AllowValidAndKnownInstances_WithNormalizeMethod
{
    // the '-1' value was removed from the known instances (above),
    // so this type demonstrates how we can normalize the input to cater for it.
    private static int NormalizeInput(int input) => input == -1 ? 0 : input;

    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}

public class Generic_DeserializationValidationTests
{
    [Theory]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("-100")]
    public void AnythingIsAllowed(string input)
    {
        var vo = CreateWith<VoGeneric_AllowAnything>(input);
        vo.Value.Should().Be(Convert.ToInt32(input));
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("0")]
    public void AllowAnything(string input)
    {
        Action a = () => CreateWith<VoGeneric_AllowValidAndKnownInstances>(input);
        a.Should().NotThrow();
    }

    [Theory]
    [InlineData("-2")]
    [InlineData("-100")]
    public void AllowValidAndKnownInstances(string input)
    {
        Action a = () => CreateWith<VoGeneric_AllowValidAndKnownInstances>(input);
        a.Should().Throw<ValueObjectValidationException>();
    }

    [Theory]
    [InlineData("-1")]
    public void AllowValidAndKnownInstances_and_run_normalize_method(string input)
    {
        var x = CreateWith<VoGeneric_AllowValidAndKnownInstances_WithNormalizeMethod>(input);
        x.Value.Should().Be(0);
    }

    private static T CreateWith<T>(string input)
    {
        TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
        object? converted = c.ConvertFrom(input);
        T t = (T)converted!;

        return t;
    }
}
#endif