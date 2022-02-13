using System;

namespace Vogen;

internal readonly struct VogenConfiguration
{
    public VogenConfiguration(
        Type validationExceptionType,
        Conversions conversions)
    {
        ValidationExceptionType = validationExceptionType;
        Conversions = conversions;
    }

    public static VogenConfiguration Combine(
        VogenConfiguration? attributeValues,
        VogenConfiguration? globalValues)
    {

        var conversions = attributeValues?.Conversions ?? globalValues?.Conversions ?? DefaultInstance.Conversions;
        var validationExceptionType = attributeValues?.ValidationExceptionType ?? globalValues?.ValidationExceptionType ?? DefaultInstance.ValidationExceptionType;

        return new VogenConfiguration(validationExceptionType, conversions);
    }

    public Type ValidationExceptionType { get; }

    public Conversions Conversions { get; }

    public static readonly VogenConfiguration DefaultInstance = new(
        validationExceptionType: typeof(ValueObjectValidationException),
        conversions: Conversions.Default);
}