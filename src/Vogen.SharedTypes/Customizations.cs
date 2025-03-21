using System;

namespace Vogen;

/// <summary>
/// Customization flags. For simple binary choices.
/// More complex configuration options are specified as parameters in the <see cref="VogenDefaultsAttribute"/>.
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
    /// When serializing and deserializing an underlying primitive that would normally be read or written as a number in System.Text.Json,
    /// instead, treat the underlying primitive as a culture invariant string. This gets around the issue of
    /// JavaScript losing precision on very large numbers. See <see href="https://github.com/SteveDunn/Vogen/issues/165"/>
    /// for more information.
    /// The preferred method, on .NET 5 or higher, is to use `JsonNumberHandling` in `JsonSerializerOptions`.
    /// </summary>
#if NET5_0_OR_GREATER
    [Obsolete("The preferred method is to use `JsonNumberHandling` in `JsonSerializerOptions`.")]
#endif    
    TreatNumberAsStringInSystemTextJson = 1 << 0,
 
    /// <summary>
    /// For GUIDs, add a `FromNewGuid()` factory method, which is just `public static MyVo FromNewGuid() => From(Guid.NewGuid());`
    /// </summary>
    AddFactoryMethodForGuids = 1 << 1,

    /// <summary>
    /// Generate ToDump for LinqPad
    /// </summary>
    GenerateLinqPadToDump = 1 << 2
}