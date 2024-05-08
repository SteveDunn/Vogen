using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Defines if a JSON converter 'factory' is generated containing value objects with System.Text.Json converters.
/// These factories can be used in various scenarios with System.Text.Json, including source-generation.
/// </summary>
public enum SystemTextJsonConverterFactoryGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    /// <summary>
    /// Do not generate the factory.
    /// </summary>
    Omit = 0,
    
    /// <summary>
    /// Generate the factory.
    /// </summary>
    Generate = 1
}