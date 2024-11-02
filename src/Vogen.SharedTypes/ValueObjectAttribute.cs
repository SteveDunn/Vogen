// ReSharper disable UnusedParameter.Local
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable UnusedType.Global

// NOTE:
// the documentation for this doesn't show up in recent (November 2024) versions of Visual Studio.
// they do show up in versions of Rider for the same period, but for the generic attribute,
// the cursor has to hover near the right bracket of the attribute! Bug filed at https://youtrack.jetbrains.com/issue/RIDER-119448/Rider-does-not-show-XML-comments-for-generic-attributes

namespace Vogen
{
    using System;

    // Generic attributes were introduced in C# 11
    /// <inheritdoc/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ValueObjectAttribute<T> : ValueObjectAttribute
    {
        // keep this signature in-line with `VogenConfiguration`
        // as the syntax/semantics are read in the generator
        // using parameter indexes (i.e. it expected param 0 to be the underlying type etc.

        /// <summary>
        /// Configures aspects of this individual value object.
        /// </summary>
        /// <param name="conversions">Specifies what conversion code is generated - defaults to <see cref="Conversions.Default"/> which generates type converters and a converter to handle serialization using System.Text.Json</param>
        /// <param name="throws">Specifies the type of exception thrown when validation fails—defaults to <see cref="ValueObjectValidationException"/>.</param>
        /// <param name="customizations">Simple customization switches—defaults to <see cref="Customizations.None"/>.</param>
        /// <param name="deserializationStrictness">Specifies how strict deserialization is, e.g. should your Validate method be called, or should pre-defined instances that otherwise invalid be allowed - defaults to <see cref="DeserializationStrictness.AllowValidAndKnownInstances"/>.</param>
        /// <param name="debuggerAttributes">Specifies the level that debug attributes are written as some IDEs don't support all of them, e.g., Rider - defaults to <see cref="DebuggerAttributeGeneration.Full"/> which generates DebuggerDisplay and a debugger proxy type for IDEs that support them</param>
        /// <param name="comparison">Species which comparison code is generated—defaults to <see cref="ComparisonGeneration.UseUnderlying"/> which hoists any IComparable implementations from the primitive.</param>
        /// <param name="stringComparers">Specifies which string comparison code is generated—defaults to <see cref="StringComparersGeneration.Omit"/> which doesn't generate anything related to string comparison.</param>
        /// <param name="toPrimitiveCasting">Controls how cast operators are generated for casting from the Value Object to the primitive.
        /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
        /// See <see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/> for more information.</param>
        /// <param name="fromPrimitiveCasting">Controls how cast operators are generated for casting from the primitive to the Value Object.
        /// Options are implicit or explicit or none.  Explicit is preferred over implicit if you really need them, but isn't recommended.
        /// See <see href="https://github.com/SteveDunn/Vogen/wiki/Casting"/> for more information.
        /// </param>
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
        public ValueObjectAttribute(
            Conversions conversions = Conversions.Default,
            Type? throws = null!,
            Customizations customizations = Customizations.None,
            DeserializationStrictness deserializationStrictness = DeserializationStrictness.AllowValidAndKnownInstances,
            DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default,
            ComparisonGeneration comparison = ComparisonGeneration.Default,
            StringComparersGeneration stringComparers = StringComparersGeneration.Unspecified,
            CastOperator toPrimitiveCasting = CastOperator.Unspecified,
            CastOperator fromPrimitiveCasting = CastOperator.Unspecified,
            ParsableForStrings parsableForStrings = ParsableForStrings.Unspecified,
            ParsableForPrimitives parsableForPrimitives = ParsableForPrimitives.Unspecified,
            TryFromGeneration tryFromGeneration = TryFromGeneration.Unspecified,
            IsInitializedMethodGeneration isInitializedMethodGeneration = IsInitializedMethodGeneration.Unspecified,
            PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.Unspecified) : base(
            typeof(T),
            conversions,
            throws,
            customizations,
            deserializationStrictness,
            debuggerAttributes,
            comparison,
            stringComparers,
            toPrimitiveCasting,
            fromPrimitiveCasting,
            parsableForStrings,
            parsableForPrimitives,
            tryFromGeneration,
            isInitializedMethodGeneration,
            primitiveEqualityGeneration)
        {
        }
    }

    /// <summary>
    /// Marks a type as a Value Object. The type that this is applied to should be partial so that the
    /// source generator can augment it with equality, creation barriers, and any conversions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ValueObjectAttribute : Attribute
    {
        // keep this signature in-line with `VogenConfiguration`
        // as the syntax/semantics are read in the generator
        // using parameter indexes (i.e. it expected param 0 to be the underlying type etc).
        
        // ReSharper disable once MemberCanBeProtected.Global
        
        /// <summary>
        /// Configures aspects of this individual value object.
        /// </summary>
        /// <param name="underlyingType">The type of the primitive that is being wrapped—defaults to int.</param>
        /// <param name="conversions">Specifies what conversion code is generated - defaults to <see cref="Conversions.Default"/> which generates type converters and a converter to handle serialization using System.Text.Json</param>
        /// <param name="throws">Specifies the type of exception thrown when validation fails—defaults to <see cref="ValueObjectValidationException"/>.</param>
        /// <param name="customizations">Simple customization switches—defaults to <see cref="Customizations.None"/>.</param>
        /// <param name="deserializationStrictness">Specifies how strict deserialization is, e.g. should your Validate method be called, or should pre-defined instances that otherwise invalid be allowed - defaults to <see cref="DeserializationStrictness.AllowValidAndKnownInstances"/>.</param>
        /// <param name="debuggerAttributes">Specifies the level that debug attributes are written as some IDEs don't support all of them, e.g., Rider - defaults to <see cref="DebuggerAttributeGeneration.Full"/> which generates DebuggerDisplay and a debugger proxy type for IDEs that support them</param>
        /// <param name="comparison">Species which comparison code is generated—defaults to <see cref="ComparisonGeneration.UseUnderlying"/> which hoists any IComparable implementations from the primitive.</param>
        /// <param name="stringComparers">Specifies which string comparison code is generated—defaults to <see cref="StringComparersGeneration.Omit"/> which doesn't generate anything related to string comparison.</param>
        /// <param name="toPrimitiveCasting">Specifies the type of casting from wrapper to primitive - defaults to <see cref="CastOperator.Explicit"/>.</param>
        /// <param name="fromPrimitiveCasting">Specifies the type of casting from primitive to wrapper - default to <see cref="CastOperator.Explicit"/>.</param>
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
        public ValueObjectAttribute(
            Type? underlyingType = null!,
            Conversions conversions = Conversions.Default,
            Type? throws = null!,
            Customizations customizations = Customizations.None,
            DeserializationStrictness deserializationStrictness = DeserializationStrictness.AllowValidAndKnownInstances,
            DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default,
            ComparisonGeneration comparison = ComparisonGeneration.Default,
            StringComparersGeneration stringComparers = StringComparersGeneration.Unspecified,
            CastOperator toPrimitiveCasting = CastOperator.Unspecified,
            CastOperator fromPrimitiveCasting = CastOperator.Unspecified,
            ParsableForStrings parsableForStrings = ParsableForStrings.Unspecified,
            ParsableForPrimitives parsableForPrimitives = ParsableForPrimitives.Unspecified,
            TryFromGeneration tryFromGeneration = TryFromGeneration.Unspecified,
            IsInitializedMethodGeneration isInitializedMethodGeneration = IsInitializedMethodGeneration.Unspecified,
            PrimitiveEqualityGeneration primitiveEqualityGeneration = PrimitiveEqualityGeneration.Unspecified)
        {
            // DO NOT ADD PARAMETERS HERE, INSTEAD, CREATE OVERLOADS (at least until a new major version).
            // This is because some users use reflection to find this attribute, and changing the amount
            // of parameters is a binary-breaking change. See https://github.com/dotnet/runtime/issues/103722
            // for more information.
        }
    }
}