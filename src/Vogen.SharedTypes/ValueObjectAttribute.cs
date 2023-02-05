// ReSharper disable UnusedParameter.Local
namespace Vogen
{
    using System;

#if NETCOREAPP
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
            DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default)
            : base(typeof(T), conversions, throws, customizations, deserializationStrictness, debuggerAttributes)
        {
        }
    }
#endif

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
        public ValueObjectAttribute(
            Type? underlyingType = null!,
            Conversions conversions = Conversions.Default,
            Type? throws = null!,
            Customizations customizations = Customizations.None,
            DeserializationStrictness deserializationStrictness = DeserializationStrictness.AllowValidAndKnownInstances,
            DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default)
        {
            UnderlyingType = underlyingType;
            // Conversions = conversions;
            // ValidationExceptionType = throws;
            // Customizations = customizations;
            // DeserializationStrictness = deserializationStrictness;
            // OmitDebugAttributes = omitDebugAttributes;
        }

        public Type? UnderlyingType { get; }
        //
        // public Type? ValidationExceptionType { get; }
        //
        // public Conversions Conversions { get; }
        //
        // public Customizations Customizations { get; }
        //
        // public DeserializationStrictness DeserializationStrictness { get; } =
        //     DeserializationStrictness.AllowValidAndKnownInstances;
        //
        // public bool OmitDebugAttributes { get; }
    }
}