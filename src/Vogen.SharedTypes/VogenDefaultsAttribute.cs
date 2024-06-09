// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedParameter.Local
namespace Vogen;

using System;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class VogenDefaultsAttribute : Attribute
{
    // ** NOTE: The default values here should be 'Unspecified' if there is such an option.
    // The default values are resolved when merging attributes, if either are unspecified,
    // then the respective value in `VogenConfiguration.DefaultInstance` is used.

    /// <summary>
    /// Creates a new instance of a type that represents the default
    /// values used for value object generation.
    /// </summary>
    /// <param name="underlyingType">The primitive underlying type.</param>
    /// <param name="conversions">Any conversions that need to be done for this type, e.g. to be serialized etc.</param>
    /// <param name="throws">The type of exception that is thrown when validation fails.</param>
    /// <param name="customizations">Any customizations, for instance, treating numbers in [de]serialization as strings.</param>
    /// <param name="deserializationStrictness">The strictness of validation when deserializing.</param>
    /// <param name="debuggerAttributes">Controls how debugger attributes are generated. This is useful in Rider where the attributes crash Rider's debugger.</param>
    /// <param name="toPrimitiveCasting">Controls how cast operators are generated for casting from the Value Object to the primitive.
    /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
    /// See <see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/> for more information.</param>
    /// <param name="fromPrimitiveCasting">Controls how cast operators are generated for casting from the primitive to the Value Object.
    /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
    /// See &lt;see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/&gt; for more information.</param>
    /// <param name="disableStackTraceRecordingInDebug">If Debug, a stack trace is recorded if something is created in an uninitialized state.
    /// This stack trace is heap based which might be unwanted if your Value Object is stack based.</param>
    /// <param name="parsableForStrings">Specifies the functionality around parsing (IParsable etc.)</param>
    /// <param name="parsableForPrimitives">Specifies the functionality around parsing (IParsable etc.)</param>
    /// <param name="tryFromGeneration">Controls what is generated for the TryFrom methods.</param>
    /// <param name="isInitializedMethodGeneration">Controls whether the IsInitialized method is generated.</param>
    /// <param name="systemTextJsonConverterFactoryGeneration">Controls the generation of the type factory for System.Text.Json.</param>
    /// <param name="staticAbstractsGeneration">Controls the generation of static abstract interfaces.</param>
    /// <param name="openApiSchemaCustomizations">Controls the generation of a Swashbuckle schema filter for OpenAPI.</param>
    /// <param name="explicitlySpecifyTypeInValueObject">Every ValueObject attribute must explicitly specify the type of the primitive.</param>
    /// <param name="primitiveEqualityGeneration">Whether or not to generate primitive comparison operators.</param>
    public VogenDefaultsAttribute(
        Type? underlyingType = null,
        Conversions conversions = Conversions.Default,
        Type? throws = null,
        Customizations customizations = Customizations.None,
        DeserializationStrictness deserializationStrictness = DeserializationStrictness.AllowValidAndKnownInstances,
        DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default,
        CastOperator toPrimitiveCasting = CastOperator.Explicit,
        CastOperator fromPrimitiveCasting = CastOperator.Explicit,
        bool disableStackTraceRecordingInDebug = false,
        ParsableForStrings parsableForStrings = ParsableForStrings.GenerateMethodsAndInterface,
        ParsableForPrimitives parsableForPrimitives = ParsableForPrimitives.HoistMethodsAndInterfaces,
        TryFromGeneration tryFromGeneration = TryFromGeneration.Unspecified,
        IsInitializedMethodGeneration isInitializedMethodGeneration = IsInitializedMethodGeneration.Unspecified,
        SystemTextJsonConverterFactoryGeneration systemTextJsonConverterFactoryGeneration =
            SystemTextJsonConverterFactoryGeneration.Unspecified,
        StaticAbstractsGeneration staticAbstractsGeneration = StaticAbstractsGeneration.Unspecified,
        OpenApiSchemaCustomizations openApiSchemaCustomizations = OpenApiSchemaCustomizations.Unspecified,
        bool explicitlySpecifyTypeInValueObject = false,
        PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.Unspecified)
    {
    }
}