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
        bool disableStackTraceRecordingInDebug,
        ParsableForStrings parsableForStrings,
        ParsableForPrimitives parsableForPrimitives,
        TryFromGeneration tryFromGeneration,
        IsInitializedMethodGeneration isInitializedMethodGeneration,
        SystemTextJsonConverterFactoryGeneration systemTextJsonConverterFactoryGeneration,
        StaticAbstractsGeneration staticAbstractsGeneration,
        SwashbuckleSchemaFilterGeneration swashbuckleSchemaFilterGeneration)
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
        ParsableForStrings = parsableForStrings;
        ParsableForPrimitives = parsableForPrimitives;
        TryFromGeneration = tryFromGeneration;
        IsInitializedMethodGeneration = isInitializedMethodGeneration;
        SystemTextJsonConverterFactoryGeneration = systemTextJsonConverterFactoryGeneration;
        StaticAbstractsGeneration = staticAbstractsGeneration;
        SwashbuckleSchemaFilterGeneration = swashbuckleSchemaFilterGeneration;
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
    
    public ParsableForStrings ParsableForStrings { get; }
    
    public ParsableForPrimitives ParsableForPrimitives { get; }

    public CastOperator ToPrimitiveCasting { get; }
    
    public CastOperator FromPrimitiveCasting { get; }
    
    public bool DisableStackTraceRecordingInDebug { get; set; }
    
    public TryFromGeneration TryFromGeneration { get; }
    public IsInitializedMethodGeneration IsInitializedMethodGeneration { get; }
    
    public SystemTextJsonConverterFactoryGeneration SystemTextJsonConverterFactoryGeneration { get; }
    
    public SwashbuckleSchemaFilterGeneration SwashbuckleSchemaFilterGeneration { get; }
    
    public StaticAbstractsGeneration StaticAbstractsGeneration { get; }

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
        disableStackTraceRecordingInDebug: false,
        parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
        parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
        tryFromGeneration: TryFromGeneration.GenerateBoolAndErrorOrMethods,
        isInitializedMethodGeneration: IsInitializedMethodGeneration.Generate,
        systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Generate,
        staticAbstractsGeneration: StaticAbstractsGeneration.Omit,
        swashbuckleSchemaFilterGeneration: SwashbuckleSchemaFilterGeneration.Omit);
}
