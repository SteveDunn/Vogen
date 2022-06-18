using System;

namespace Vogen;

/// <summary>
/// Customization flags. For things like treating doubles as strings
/// during [de]serialization (for compatibility with JavaScript).
/// </summary>
[Flags]
public enum Customizations
{
    // Used with HasFlag, so needs to be 1, 2, 4 etc

    /// <summary>
    /// No customizations.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// When [de]serializing an underlying primitive that wold normally be written as a number in System.Text.Json,
    /// instead, treat the underlying primitive as a culture invariant string. This gets around the issue of
    /// JavaScript losing precision on very large numbers. See <see href="https://github.com/SteveDunn/Vogen/issues/165"/>
    /// for more information.
    /// </summary>
    TreatNumberAsStringInSystemTextJson = 1 << 0
}