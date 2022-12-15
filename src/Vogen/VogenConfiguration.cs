using System;
using Microsoft.CodeAnalysis;

namespace Vogen;

public readonly struct VogenConfiguration
{
    public VogenConfiguration(INamedTypeSymbol? underlyingType,
        INamedTypeSymbol? validationExceptionType,
        Conversions conversions,
        Customizations customizations,
        DeserializationStrictness deserializationStrictness,
        bool? omitDebugAttributes)
    {
        UnderlyingType = underlyingType;
        ValidationExceptionType = validationExceptionType;
        Conversions = conversions;
        Customizations = customizations;
        DeserializationStrictness = deserializationStrictness;
        OmitDebugAttributes = omitDebugAttributes;
    }

    public static VogenConfiguration Combine(
        VogenConfiguration localValues,
        VogenConfiguration? globalValues,
        Func<INamedTypeSymbol>? funcForDefaultUnderlyingType = null)
    {
        var conversions = (localValues.Conversions, globalValues?.Conversions) switch
        {
            (Conversions.Default, null) => DefaultInstance.Conversions,
            (Conversions.Default, Conversions.Default) => DefaultInstance.Conversions,
            (Conversions.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var customizations = (localValues.Customizations, globalValues?.Customizations) switch
        {
            (Customizations.None, null) => DefaultInstance.Customizations,
            (Customizations.None, Customizations.None) => DefaultInstance.Customizations,
            (Customizations.None, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var strictness = (localValues.DeserializationStrictness, globalValues?.DeserializationStrictness) switch
        {
            (DeserializationStrictness.Default, null) => DefaultInstance.DeserializationStrictness,
            (DeserializationStrictness.Default, DeserializationStrictness.Default) => DefaultInstance.DeserializationStrictness,
            (DeserializationStrictness.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var validationExceptionType = localValues.ValidationExceptionType ?? globalValues?.ValidationExceptionType ?? DefaultInstance.ValidationExceptionType;
        var underlyingType = localValues.UnderlyingType ?? globalValues?.UnderlyingType ?? funcForDefaultUnderlyingType?.Invoke();
        var omitDebugAttributes = localValues.OmitDebugAttributes ?? globalValues?.OmitDebugAttributes ?? false;

        return new VogenConfiguration(underlyingType, validationExceptionType, conversions, customizations, strictness, omitDebugAttributes);
    }

    public INamedTypeSymbol? UnderlyingType { get; }
    
    public INamedTypeSymbol? ValidationExceptionType { get; }

    public Conversions Conversions { get; }
    
    public Customizations Customizations { get; }
    public DeserializationStrictness DeserializationStrictness { get; }
    
    public bool? OmitDebugAttributes { get; }

    // the issue here is that without a physical 'symbol' in the source, we can't
    // get the namedtypesymbol
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly VogenConfiguration DefaultInstance = new(
        underlyingType: null,
        validationExceptionType: null,
        // ReSharper disable once RedundantNameQualifier
        conversions: Vogen.Conversions.Default,
        customizations: Customizations.None,
        deserializationStrictness: DeserializationStrictness.Default,
        omitDebugAttributes: false);
}
