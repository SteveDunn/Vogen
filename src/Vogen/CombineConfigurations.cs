using System;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class CombineConfigurations
{
    // Combines two configurations, choosing the values in the first one over the values in the second one.
    // If the second one is null, then the default instance value is used.
    // If the values in the left and right are unspecified, then the value in the default instance is used.

    // This is for to combine the configuration of global and local config, where local
    // config is guaranteed to be present, but the global config was not.
    
    // It is also used to combine global config with the default instance; before building the value objects,
    // we build global config first to see if the source generator needs to generate one-off source, like the System.Text.Json
    // type factories, or the static abstract interfaces. In this scenario, there is no local config to consider,
    // but we still need to combine any configuration provided with the default instance.
    public static VogenConfiguration CombineAndResolveAnythingUnspecified(
        VogenConfiguration localValues,
        VogenConfiguration? globalValues,
        Func<INamedTypeSymbol>? funcForDefaultUnderlyingType = null)
    {
        var conversions = (localValues.Conversions, globalValues?.Conversions) switch
        {
            (Conversions.Default, null) => VogenConfiguration.DefaultInstance.Conversions,
            (Conversions.Default, Conversions.Default) => VogenConfiguration.DefaultInstance.Conversions,
            (Conversions.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var customizations = (localValues.Customizations, globalValues?.Customizations) switch
        {
            (Customizations.None, null) => VogenConfiguration.DefaultInstance.Customizations,
            (Customizations.None, Customizations.None) => VogenConfiguration.DefaultInstance.Customizations,
            (Customizations.None, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var strictness = (localValues.DeserializationStrictness, globalValues?.DeserializationStrictness) switch
        {
            (DeserializationStrictness.Default, null) => VogenConfiguration.DefaultInstance.DeserializationStrictness,
            (DeserializationStrictness.Default, DeserializationStrictness.Default) => VogenConfiguration.DefaultInstance.DeserializationStrictness,
            (DeserializationStrictness.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var debuggerAttributes = (localValues.DebuggerAttributes, globalValues?.DebuggerAttributes) switch
        {
            (DebuggerAttributeGeneration.Default, null) => VogenConfiguration.DefaultInstance.DebuggerAttributes,
            (DebuggerAttributeGeneration.Default, DebuggerAttributeGeneration.Default) => VogenConfiguration.DefaultInstance.DebuggerAttributes,
            (DebuggerAttributeGeneration.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var comparison = (localValues.Comparison, globalValues?.Comparison) switch
        {
            (ComparisonGeneration.Default, null) => VogenConfiguration.DefaultInstance.Comparison,
            (ComparisonGeneration.Default, ComparisonGeneration.Default) => VogenConfiguration.DefaultInstance.Comparison,
            (ComparisonGeneration.Default, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var parsableForStrings = (localValues.ParsableForStrings, globalValues?.ParsableForStrings) switch
        {
            (ParsableForStrings.Unspecified, null) => VogenConfiguration.DefaultInstance.ParsableForStrings,
            (ParsableForStrings.Unspecified, ParsableForStrings.Unspecified) => VogenConfiguration.DefaultInstance.ParsableForStrings,
            (ParsableForStrings.Unspecified, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        var parsableForPrimitives = (localValues.ParsableForPrimitives, globalValues?.ParsableForPrimitives) switch
        {
            (ParsableForPrimitives.Unspecified, null) => VogenConfiguration.DefaultInstance.ParsableForPrimitives,
            (ParsableForPrimitives.Unspecified, ParsableForPrimitives.Unspecified) => VogenConfiguration.DefaultInstance.ParsableForPrimitives,
            (ParsableForPrimitives.Unspecified, var globalDefault) => globalDefault.Value,
            (var specificValue, _) => specificValue
        };

        StringComparersGeneration stringComparers = (localValues.StringComparers, globalValues?.StringComparers) switch
        {
            (StringComparersGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.StringComparers,
            (StringComparersGeneration.Unspecified, StringComparersGeneration.Unspecified) => VogenConfiguration.DefaultInstance.StringComparers,
            (StringComparersGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        TryFromGeneration tryFromGeneration = (localValues.TryFromGeneration, globalValues?.TryFromGeneration) switch
        {
            (TryFromGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.TryFromGeneration,
            (TryFromGeneration.Unspecified, TryFromGeneration.Unspecified) => VogenConfiguration.DefaultInstance.TryFromGeneration,
            (TryFromGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        CastOperator toPrimitiveCastOperators = (localValues.ToPrimitiveCasting, globalValues?.ToPrimitiveCasting) switch
        {
            (CastOperator.Unspecified, null) => VogenConfiguration.DefaultInstance.ToPrimitiveCasting,
            (CastOperator.Unspecified, CastOperator.Unspecified) => VogenConfiguration.DefaultInstance.ToPrimitiveCasting,
            (CastOperator.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        CastOperator fromPrimitiveCastOperators = (localValues.FromPrimitiveCasting, globalValues?.FromPrimitiveCasting) switch
        {
            (CastOperator.Unspecified, null) => VogenConfiguration.DefaultInstance.FromPrimitiveCasting,
            (CastOperator.Unspecified, CastOperator.Unspecified) => VogenConfiguration.DefaultInstance.FromPrimitiveCasting,
            (CastOperator.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        IsInitializedMethodGeneration isInitializedMethodGeneration = (localValues.IsInitializedMethodGeneration, globalValues?.IsInitializedMethodGeneration) switch
        {
            (IsInitializedMethodGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.IsInitializedMethodGeneration,
            (IsInitializedMethodGeneration.Unspecified, IsInitializedMethodGeneration.Unspecified) => VogenConfiguration.DefaultInstance.IsInitializedMethodGeneration,
            (IsInitializedMethodGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        SystemTextJsonConverterFactoryGeneration stjFactories = 
            (localValues.SystemTextJsonConverterFactoryGeneration, globalValues?.SystemTextJsonConverterFactoryGeneration) switch
            {
                (SystemTextJsonConverterFactoryGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.SystemTextJsonConverterFactoryGeneration,
                (SystemTextJsonConverterFactoryGeneration.Unspecified, SystemTextJsonConverterFactoryGeneration.Unspecified) => VogenConfiguration.DefaultInstance.SystemTextJsonConverterFactoryGeneration,
                (SystemTextJsonConverterFactoryGeneration.Unspecified, var global) => global.Value,
                (var local, _) => local,
            };

        StaticAbstractsGeneration staticAbstractsGeneration = (localValues.StaticAbstractsGeneration, globalValues?.StaticAbstractsGeneration) switch
        {
            (StaticAbstractsGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.StaticAbstractsGeneration,
            (StaticAbstractsGeneration.Unspecified, StaticAbstractsGeneration.Unspecified) => VogenConfiguration.DefaultInstance.StaticAbstractsGeneration,
            (StaticAbstractsGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        SwashbuckleSchemaGeneration swashbuckleSchemaGeneration = (localValues.SwashbuckleSchemaGeneration, globalValues?.SwashbuckleSchemaGeneration) switch
        {
            (SwashbuckleSchemaGeneration.Unspecified, null) => VogenConfiguration.DefaultInstance.SwashbuckleSchemaGeneration,
            (SwashbuckleSchemaGeneration.Unspecified, SwashbuckleSchemaGeneration.Unspecified) => VogenConfiguration.DefaultInstance.SwashbuckleSchemaGeneration,
            (SwashbuckleSchemaGeneration.Unspecified, var global) => global.Value,
            (var local, _) => local,
        };

        var validationExceptionType = localValues.ValidationExceptionType ?? 
                                      globalValues?.ValidationExceptionType ?? 
                                      VogenConfiguration.DefaultInstance.ValidationExceptionType;

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
            disableStackTraceRecordingInDebug: disableStackTraceRecordingInDebug,
            parsableForStrings: parsableForStrings,
            parsableForPrimitives: parsableForPrimitives,
            tryFromGeneration: tryFromGeneration,
            isInitializedMethodGeneration: isInitializedMethodGeneration,
            systemTextJsonConverterFactoryGeneration: stjFactories,
            staticAbstractsGeneration: staticAbstractsGeneration,
            swashbuckleSchemaGeneration: swashbuckleSchemaGeneration);
    }
 
    /// If we don't have a global attribute, just use the default configuration as there
    /// is nothing to combine (i.e. there's no local config to combine it with),
    /// otherwise, combine what we're given *with* the default instance to resolve
    /// any unspecified arguments
    public static VogenConfiguration CombineAndResolveAnyGlobalConfig(VogenConfiguration? config) => 
        config is null ? VogenConfiguration.DefaultInstance : CombineAndResolveAnythingUnspecified(config, VogenConfiguration.DefaultInstance);

}