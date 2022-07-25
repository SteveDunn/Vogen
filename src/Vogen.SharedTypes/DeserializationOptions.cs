using System;

namespace Vogen;

[Flags]
public enum DeserializationStrictness
{
    /// <summary>
    /// Allows anything to be deserialized into your Value Object.
    /// </summary>
    AllowAnything = 0,

    /// <summary>
    /// If your Value Object has 'Instances', then those are considered valid during deserialization.
    /// </summary>
    AllowKnownInstances = 1 << 0,

    /// <summary>
    /// If your Value Object has a Validate method, it will be called to validate the incoming value during deserialization.
    /// </summary>
    RunMyValidationMethod = 1 << 1,

    /// <summary>
    /// If your Value Object has 'Instances', then those are considered valid during deserialization.
    /// If the incoming value doesn't match any known instance, and if your Value Object has a Validate method, it will
    /// be called to validate the incoming value during deserialization. 
    /// </summary>
    AllowValidAndKnownInstances = AllowKnownInstances | RunMyValidationMethod,

    Default = AllowValidAndKnownInstances,
}