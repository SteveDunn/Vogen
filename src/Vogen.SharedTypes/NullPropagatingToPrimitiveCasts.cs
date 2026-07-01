using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Controls whether the generated <b>explicit</b> to-primitive cast on a reference-type value object
/// propagates <see langword="null"/> instead of throwing.
/// <para>
/// For a class-based value object, the plain cast dereferences the receiver
/// (<c>value.Value</c>), so casting a <see langword="null"/> reference throws a
/// <see cref="System.NullReferenceException"/>. A null-propagating cast instead returns
/// <see langword="null"/> for a <see langword="null"/> receiver, mirroring how C# lifts
/// user-defined conversions over <see cref="System.Nullable{T}"/> for struct-based value objects.
/// </para>
/// <para>
/// This affects the explicit cast only; implicit to-primitive casts are left unchanged. It applies to
/// reference-type wrappers in a nullable-enabled context, is a no-op for structs (which C# already
/// lifts), and is silently ignored when it would conflict with static-abstract cast generation.
/// </para>
/// </summary>
public enum NullPropagatingToPrimitiveCasts
{
    /// <summary>
    /// Use the default behaviour: generate a null-propagating cast for wrappers whose underlying
    /// type is a reference type (e.g. <see cref="string"/>), but not for value-type underlyings
    /// (whose cast result type would otherwise change from, say, <c>int</c> to <c>int?</c>).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,

    /// <summary>
    /// Never generate a null-propagating cast. The to-primitive cast throws when the receiver is
    /// <see langword="null"/> (the behaviour prior to v9).
    /// </summary>
    Omit = 0,

    /// <summary>
    /// Generate a null-propagating to-primitive cast for all underlying types, including value types
    /// (whose cast result type becomes nullable, e.g. <c>int?</c>), returning <see langword="null"/>
    /// for a <see langword="null"/> receiver.
    /// </summary>
    Generate
}
