using System;

namespace Vogen;

public readonly struct VogenConfiguration
{
    public VogenConfiguration(
        Type? underlyingType,
        Type? validationExceptionType,
        Conversions? conversions)
    {
        UnderlyingType = underlyingType;
        ValidationExceptionType = validationExceptionType;
        Conversions = conversions;
    }

    public static VogenConfiguration Combine(
        VogenConfiguration localValues,
        VogenConfiguration? globalValues)
    {

        Conversions? conversions = localValues.Conversions ?? globalValues?.Conversions ?? DefaultInstance.Conversions;
        var validationExceptionType = localValues.ValidationExceptionType ?? globalValues?.ValidationExceptionType ?? DefaultInstance.ValidationExceptionType;
        var underlyingType = localValues.UnderlyingType ?? globalValues?.UnderlyingType ?? DefaultInstance.UnderlyingType;

        return new VogenConfiguration(underlyingType, validationExceptionType, conversions);
    }

    public Type? UnderlyingType { get; }
    
    public Type? ValidationExceptionType { get; }

    public Conversions? Conversions { get; }

    public static readonly VogenConfiguration DefaultInstance = new(
        underlyingType: typeof(int),
        validationExceptionType: typeof(ValueObjectValidationException),
        conversions: Vogen.Conversions.Default);
}