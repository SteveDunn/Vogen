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
    PrimitiveEqualityGeneration PrimitiveEqualityGeneration,
    NumericsGeneration NumericsGeneration,
    StringComparisonDefault StringDefaultComparison,
    NullPropagatingToPrimitiveCasts NullPropagatingToPrimitiveCasts)
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
        PrimitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods,
        NumericsGeneration: NumericsGeneration.Omit,
        StringDefaultComparison: StringComparisonDefault.Omit,
        // Left Unspecified: the effective default is conditional on the underlying type, resolved in
        // GenerateCodeForCastingOperators.
        NullPropagatingToPrimitiveCasts: NullPropagatingToPrimitiveCasts.Unspecified);

    public string? GetStringDefaultComparerExpression() => StringDefaultComparison switch
    {
        StringComparisonDefault.Ordinal => "global::System.StringComparer.Ordinal",
        StringComparisonDefault.OrdinalIgnoreCase => "global::System.StringComparer.OrdinalIgnoreCase",
        StringComparisonDefault.CurrentCulture => "global::System.StringComparer.CurrentCulture",
        StringComparisonDefault.CurrentCultureIgnoreCase => "global::System.StringComparer.CurrentCultureIgnoreCase",
        StringComparisonDefault.InvariantCulture => "global::System.StringComparer.InvariantCulture",
        StringComparisonDefault.InvariantCultureIgnoreCase => "global::System.StringComparer.InvariantCultureIgnoreCase",
        _ => null
    };
}
