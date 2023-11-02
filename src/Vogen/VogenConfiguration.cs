using System;
using Microsoft.CodeAnalysis;

namespace Vogen;

public class VogenConfiguration
{
    // Don't add default values here, they should be in DefaultInstance.
    public VogenConfiguration(
        INamedTypeSymbol? underlyingType,
        INamedTypeSymbol? validationExceptionType,
        Conversions conversions,
        Customizations customizations,
        DeserializationStrictness deserializationStrictness,
        DebuggerAttributeGeneration debuggerAttributes,
        ComparisonGeneration comparison,
        StringComparersGeneration stringComparers,
        CastOperator toPrimitiveCasting,
        CastOperator fromPrimitiveCasting,
        bool disableStackTraceRecordingInDebug)
    {
        UnderlyingType = underlyingType;
        ValidationExceptionType = validationExceptionType;
        Conversions = conversions;
        Customizations = customizations;
        DeserializationStrictness = deserializationStrictness;
        DebuggerAttributes = debuggerAttributes;
        Comparison = comparison;
        StringComparers = stringComparers;
        ToPrimitiveCasting = toPrimitiveCasting;
        FromPrimitiveCasting = fromPrimitiveCasting;
        DisableStackTraceRecordingInDebug = disableStackTraceRecordingInDebug;
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

        var debuggerAttributes = (localValues.DebuggerAttributes, globalValues?.DebuggerAttributes) switch
        {
            (DebuggerAttributeGeneration.Default, null) => DefaultInstance.DebuggerAttributes,
            (DebuggerAttributeGeneration.Default, DebuggerAttributeGeneration.Default) => DefaultInstance.DebuggerAttributes,
            (DebuggerAttributeGeneration.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var comparison = (localValues.Comparison, globalValues?.Comparison) switch
        {
            (ComparisonGeneration.Default, null) => DefaultInstance.Comparison,
            (ComparisonGeneration.Default, ComparisonGeneration.Default) => DefaultInstance.Comparison,
            (ComparisonGeneration.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        StringComparersGeneration stringComparers = (localValues.StringComparers, globalValues?.StringComparers) switch
        {
            (StringComparersGeneration.Unspecified, null) => DefaultInstance.StringComparers,
            (StringComparersGeneration.Unspecified, StringComparersGeneration.Unspecified) => DefaultInstance.StringComparers,
            (StringComparersGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        CastOperator toPrimitiveCastOperators = (localValues.ToPrimitiveCasting, globalValues?.ToPrimitiveCasting) switch
        {
            (CastOperator.Unspecified, null) => DefaultInstance.ToPrimitiveCasting,
            (CastOperator.Unspecified, CastOperator.Unspecified) => DefaultInstance.ToPrimitiveCasting,
            (CastOperator.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        CastOperator fromPrimitiveCastOperators = (localValues.FromPrimitiveCasting, globalValues?.FromPrimitiveCasting) switch
        {
            (CastOperator.Unspecified, null) => DefaultInstance.FromPrimitiveCasting,
            (CastOperator.Unspecified, CastOperator.Unspecified) => DefaultInstance.FromPrimitiveCasting,
            (CastOperator.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        var validationExceptionType = localValues.ValidationExceptionType ?? 
                                      globalValues?.ValidationExceptionType ?? 
                                      DefaultInstance.ValidationExceptionType;

        var underlyingType = localValues.UnderlyingType ?? 
                             globalValues?.UnderlyingType ?? 
                             funcForDefaultUnderlyingType?.Invoke();
        
        var disableStackTraceRecordingInDebug = globalValues?.DisableStackTraceRecordingInDebug ?? false;

        return new VogenConfiguration(
            underlyingType: underlyingType,
            validationExceptionType: validationExceptionType,
            conversions: conversions,
            customizations: customizations,
            deserializationStrictness: strictness,
            debuggerAttributes: debuggerAttributes,
            comparison: comparison,
            stringComparers: stringComparers,
            toPrimitiveCasting: toPrimitiveCastOperators,
            fromPrimitiveCasting: fromPrimitiveCastOperators,
            disableStackTraceRecordingInDebug: disableStackTraceRecordingInDebug);
    }

    /// <summary>
    /// The underlying type. It may be null if not specified (i.e. defaulted).
    /// </summary>
    public INamedTypeSymbol? UnderlyingType { get; }
    
    public INamedTypeSymbol? ValidationExceptionType { get; }

    public Conversions Conversions { get; }
    
    public Customizations Customizations { get; }
    public DeserializationStrictness DeserializationStrictness { get; }
    
    public DebuggerAttributeGeneration DebuggerAttributes { get; }
    
    public ComparisonGeneration Comparison { get; }
    
    public StringComparersGeneration StringComparers { get; }

    public CastOperator ToPrimitiveCasting { get; }
    
    public CastOperator FromPrimitiveCasting { get; }
    
    public bool DisableStackTraceRecordingInDebug { get; set; }

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
        debuggerAttributes: DebuggerAttributeGeneration.Full,
        comparison: ComparisonGeneration.UseUnderlying,
        stringComparers: StringComparersGeneration.Omit,
        toPrimitiveCasting: CastOperator.Explicit,
        fromPrimitiveCasting: CastOperator.Explicit,
        disableStackTraceRecordingInDebug: false);
}
