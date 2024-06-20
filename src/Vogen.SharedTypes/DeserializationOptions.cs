using System;

namespace Vogen;

[Flags]
public enum DeserializationStrictness
{
    /// <summary>
    /// Allows anything to be deserialized into your Value Object.
    /// </summary>
#pragma warning disable CA1008 // Enums should have zero value
    AllowAnything = 0,
#pragma warning restore CA1008 // Enums should have zero value

    /// <summary>
    /// If your Value Object has 'Instances', then those are considered valid during deserialization.
    /// </summary>
    AllowKnownInstances = 1 << 0,

    /// <summary>
    /// If your Value Object has a Validate method, it will be called to validate the incoming value during deserialization.
    /// </summary>
    RunMyValidationMethod = 1 << 1,

    /// <summary>
    /// System.Text.Json only for now. By default, System.Text.Json allows nulls for reference types.
    /// If you want to disallow this behaviour for value objects, then set this flag. 
    /// </summary>
    DisallowNulls = 1 << 2,

    /// <summary>
    /// If your Value Object has 'Instances', then those are considered valid during deserialization.
    /// If the incoming value doesn't match any known instance, and if your Value Object has a Validate method, it will
    /// be called to validate the incoming value during deserialization. 
    /// </summary>
    AllowValidAndKnownInstances = AllowKnownInstances | RunMyValidationMethod,

    Default = AllowValidAndKnownInstances,
}