
#if !NETSTANDARD || !NETCOREAPP

namespace System.Diagnostics.CodeAnalysis;

#if DEBUG
/// <summary>
///     Specifies that when a method returns <see cref="ReturnValue"/>,
///     the parameter will not be <see langword="null"/> even if the corresponding type allows it.
/// </summary>
#endif
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
#endif
internal sealed class NotNullWhenAttribute : Attribute
{
#if DEBUG
    /// <summary>
    ///     Gets the return value condition.
    ///     If the method returns this value, the associated parameter will not be <see langword="null"/>.
    /// </summary>
#endif
    public bool ReturnValue { get; }

#if DEBUG
    /// <summary>
    ///     Initializes the attribute with the specified return value condition.
    /// </summary>
    /// <param name="returnValue">
    ///     The return value condition.
    ///     If the method returns this value, the associated parameter will not be <see langword="null"/>.
    /// </param>
#endif
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}

#if DEBUG
/// <summary>
///     Specifies that <see langword="null"/> is allowed as an input even if the
///     corresponding type disallows it.
/// </summary>
#endif
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property,
    Inherited = false
)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
#endif
internal sealed class AllowNullAttribute : Attribute
{
#if DEBUG
    /// <summary>
    ///     Initializes a new instance of the <see cref="AllowNullAttribute"/> class.
    /// </summary>
#endif
    public AllowNullAttribute() { }
}


#endif