using System;
using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Defines if equality operators to the underlying primitives are generated.
/// </summary>
[Flags]
public enum PrimitiveEqualityGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,

    /// <summary>
    /// Do not generate.
    /// </summary>
    Omit = 0,

    /// <summary>
    /// Generate equals operators for primitives.
    /// </summary>
    GenerateOperators = 1 << 0,

    /// <summary>
    /// Generate equals methods for primitives.
    /// </summary>
    GenerateMethods = 1 << 1,
    
    /// <summary>
    /// Generate both operators and methods.
    /// </summary>
    GenerateOperatorsAndMethods = GenerateOperators | GenerateMethods
}