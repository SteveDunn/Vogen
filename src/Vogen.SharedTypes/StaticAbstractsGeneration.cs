using System;
using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Defines if static abstract interfaces should be generated.
/// </summary>
[Flags]
public enum StaticAbstractsGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    /// <summary>
    /// Do not generate the factory.
    /// </summary>
    Omit = 0,
    
    /// <summary>
    /// Generate the explicit casting operators from primitives.
    /// </summary>
    ExplicitCastFromPrimitive = 1 << 0,

    /// <summary>
    /// Generate the explicit casting operators to primitives.
    /// </summary>
    ExplicitCastToPrimitive = 1 << 1,

    /// <summary>
    /// Generate the implicit casting operators from primitives.
    /// </summary>
    ImplicitCastFromPrimitive = 1 << 2,

    /// <summary>
    /// Generate the implicit casting operators to primitives.
    /// </summary>
    ImplicitCastToPrimitive = 1 << 3,

    /// <summary>
    /// Generate the equals operators.
    /// </summary>
    EqualsOperators = 1 << 4,

    /// <summary>
    /// Generate the factory methods (From/TryFrom).
    /// </summary>
    FactoryMethods = 1 << 5,

    /// <summary>
    /// Generate the instance methods, e.g. Value and IsInitialized
    /// </summary>
    InstanceMethodsAndProperties = 1 << 5,
}