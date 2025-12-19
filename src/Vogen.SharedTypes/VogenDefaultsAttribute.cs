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
    /// <param name="underlyingType">The type of the primitive that is being wrapped—defaults to int.</param>
    /// <param name="conversions">Specifies what conversion code is generated - defaults to <see cref="Conversions.Default"/> which generates type converters and a converter to handle serialization using System.Text.Json</param>
    /// <param name="throws">Specifies the type of exception thrown when validation fails—defaults to <see cref="ValueObjectValidationException"/>.</param>
    /// <param name="customizations">Simple customization switches—defaults to <see cref="Customizations.None"/>.</param>
    /// <param name="deserializationStrictness">Specifies how strict deserialization is, e.g. should your Validate method be called, or should pre-defined instances that otherwise invalid be allowed - defaults to <see cref="DeserializationStrictness.AllowValidAndKnownInstances"/>.</param>
    /// <param name="debuggerAttributes">Specifies the level that debug attributes are written as some IDEs don't support all of them, e.g., Rider - defaults to <see cref="DebuggerAttributeGeneration.Full"/> which generates DebuggerDisplay and a debugger proxy type for IDEs that support them</param>
    /// <param name="toPrimitiveCasting">Controls how cast operators are generated for casting from the Value Object to the primitive.
    /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
    /// See <see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/> for more information.</param>
    /// <param name="fromPrimitiveCasting">Controls how cast operators are generated for casting from the primitive to the Value Object.
    /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
    /// See &lt;see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/&gt; for more information.</param>
    /// <param name="disableStackTraceRecordingInDebug">disables stack trace recording; in Debug builds, a stack trace is recorded and is
    /// thrown in the exception when something is created in an uninitialized state, e.g. after deserialization</param>
    /// <param name="parsableForStrings">Specifies what is generated for IParsable types for strings - defaults to <see cref="ParsableForStrings.GenerateMethodsAndInterface"/>.</param>
    /// <param name="parsableForPrimitives">Specifies what is generated for Parse and TryParse methods - defaults to <see cref="ParsableForPrimitives.HoistMethodsAndInterfaces"/>.</param>
    /// <param name="tryFromGeneration">Specifies what to write for TryFrom methods—defaults to <see cref="TryFromGeneration.GenerateBoolAndErrorOrMethods"/>.</param>
    /// <param name="isInitializedMethodGeneration">Specifies whether to generate an IsInitialized() method - defaults to <see cref="IsInitializedMethodGeneration.Generate"/>.</param>
    /// <param name="primitiveEqualityGeneration">
    /// Specifies whether to generate primitive comparison operators, allowing this type to be compared for equality to the primitive.
    /// Defaults to <see cref="PrimitiveEqualityGeneration.GenerateOperatorsAndMethods"/>
    /// <example>
    /// <para>
    /// var vo = MyInt.From(123);
    /// </para>
    /// <para>
    /// bool same = vo == 123;
    /// </para>
    /// </example>
    /// </param>
    /// <param name="systemTextJsonConverterFactoryGeneration">Controls the generation of the type factory for System.Text.Json.</param>
    /// <param name="staticAbstractsGeneration">Controls the generation of static abstract interfaces.</param>
    /// <param name="openApiSchemaCustomizations">Controls the generation of a Swashbuckle schema filter for OpenAPI.</param>
    /// <param name="explicitlySpecifyTypeInValueObject">Every ValueObject attribute must explicitly specify the type of the primitive.</param>
    public VogenDefaultsAttribute(
        Type? underlyingType = null,
        Conversions conversions = Conversions.Unspecified,
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