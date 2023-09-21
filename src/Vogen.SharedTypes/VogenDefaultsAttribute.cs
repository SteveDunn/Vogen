// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedParameter.Local
namespace Vogen;

using System;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class VogenDefaultsAttribute : Attribute
{
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
    public VogenDefaultsAttribute(
        Type? underlyingType = null,
        Conversions conversions = Conversions.Default,
        Type? throws = null,
        Customizations customizations = Customizations.None,
        DeserializationStrictness deserializationStrictness = DeserializationStrictness.AllowValidAndKnownInstances,
        DebuggerAttributeGeneration debuggerAttributes = DebuggerAttributeGeneration.Default)
    {
    }
}