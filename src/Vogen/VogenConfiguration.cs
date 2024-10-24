using Microsoft.CodeAnalysis;

namespace Vogen;

public record VogenConfiguration(
    INamedTypeSymbol? UnderlyingType,
    INamedTypeSymbol? ValidationExceptionType,
    Conversions Conversions,
    Customizations Customizations,
    DeserializationStrictness DeserializationStrictness,
    DebuggerAttributeGeneration DebuggerAttributes,
    ComparisonGeneration Comparison,
    StringComparersGeneration StringComparers,
    CastOperator ToPrimitiveCasting,
    CastOperator FromPrimitiveCasting,
    bool DisableStackTraceRecordingInDebug,
    ParsableForStrings ParsableForStrings,
    ParsableForPrimitives ParsableForPrimitives,
    TryFromGeneration TryFromGeneration,
    IsInitializedMethodGeneration IsInitializedMethodGeneration,
    SystemTextJsonConverterFactoryGeneration SystemTextJsonConverterFactoryGeneration,
    StaticAbstractsGeneration StaticAbstractsGeneration,
    OpenApiSchemaCustomizations OpenApiSchemaCustomizations,
    bool ExplicitlySpecifyTypeInValueObject,
    PrimitiveEqualityGeneration PrimitiveEqualityGeneration)
{
    // Don't add default values here, they should be in DefaultInstance.

    // the issue here is that without a physical 'symbol' in the source, we can't
    // get the namedtypesymbol
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly VogenConfiguration DefaultInstance = new(
        UnderlyingType: null,
        ValidationExceptionType: null,
        // ReSharper disable once RedundantNameQualifier
        Conversions: Vogen.Conversions.Default,
        Customizations: Customizations.None,
        DeserializationStrictness: DeserializationStrictness.Default,
        DebuggerAttributes: DebuggerAttributeGeneration.Full,
        Comparison: ComparisonGeneration.UseUnderlying,
        StringComparers: StringComparersGeneration.Omit,
        ToPrimitiveCasting: CastOperator.Explicit,
        FromPrimitiveCasting: CastOperator.Explicit,
        DisableStackTraceRecordingInDebug: false,
        ParsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
        ParsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
        TryFromGeneration: TryFromGeneration.GenerateBoolAndErrorOrMethods,
        IsInitializedMethodGeneration: IsInitializedMethodGeneration.Generate,
        SystemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Generate,
        StaticAbstractsGeneration: StaticAbstractsGeneration.Omit,
        OpenApiSchemaCustomizations: OpenApiSchemaCustomizations.Omit,
        ExplicitlySpecifyTypeInValueObject: false,
        PrimitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
}
