// ReSharper disable UnusedParameter.Local
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable UnusedType.Global
namespace Vogen
{
    using System;

    // Generic attributes were introduced in .NET 5 and C# 9
    /// <summary>
    /// Marks a type as a Value Object. The type should be partial so that the
    /// source generator can augment the type with equality and validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ValueObjectAttribute<T> : ValueObjectAttribute
    {
        // keep this signature in-line with `VogenConfiguration`
        // as the syntax/semantics are read in the generator
        // using parameter indexes (i.e. it expected param 0 to be the underlying type etc.
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
            IsInitializedGeneration isInitializedGeneration = IsInitializedGeneration.Unspecified) : base(
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
            isInitializedGeneration)
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
            IsInitializedGeneration isInitializedGeneration = IsInitializedGeneration.Unspecified)
        {
        }
    }
}