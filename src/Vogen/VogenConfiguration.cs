using Microsoft.CodeAnalysis;

namespace Vogen;

public readonly struct VogenConfiguration
{
    public VogenConfiguration(
        INamedTypeSymbol? underlyingType,
        INamedTypeSymbol? validationExceptionType,
        Conversions conversions)
    {
        UnderlyingType = underlyingType;
        ValidationExceptionType = validationExceptionType;
        Conversions = conversions;
    }

    public static VogenConfiguration Combine(
        VogenConfiguration localValues,
        VogenConfiguration? globalValues)
    {
        var conversions = (localValues.Conversions, globalValues?.Conversions) switch
        {
            (Conversions.Default, null) => DefaultInstance.Conversions,
            (Conversions.Default, Conversions.Default) => DefaultInstance.Conversions,
            (Conversions.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };


        var validationExceptionType = localValues.ValidationExceptionType ?? globalValues?.ValidationExceptionType ?? DefaultInstance.ValidationExceptionType;
        var underlyingType = localValues.UnderlyingType ?? globalValues?.UnderlyingType ?? DefaultInstance.UnderlyingType;

        return new VogenConfiguration(underlyingType, validationExceptionType, conversions);
    }

    public INamedTypeSymbol? UnderlyingType { get; }
    
    public INamedTypeSymbol? ValidationExceptionType { get; }

    public Conversions Conversions { get; }

    // the issue here is that without a physical 'symbol' in the source, we can't
    // get the namedtypesymbol
    public static readonly VogenConfiguration DefaultInstance = new(
        underlyingType: null,
        validationExceptionType: null,
        conversions: Vogen.Conversions.Default);
}
