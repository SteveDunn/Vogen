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
    /// Do not generate the interface definition, nor make the value objects derive implement the interface.
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
    InstanceMethodsAndProperties = 1 << 6,

    /// <summary>
    /// Value objects that are generated implement the generic `: IVogen` interface.
    /// If you don't want the interface declaration to be generated, e.g., because you're declaring it in another
    /// project (either Vogen is generating it, or you've declared it yourself), combine this with <see cref="OmitInterfaceDeclaration"/>.
    /// NOTE: Use the more specifically named 'InstancesHaveInterfaceDefinition' instead - this will be marked as obsolete at some point.
    /// </summary>
    ValueObjectsDeriveFromTheInterface = 1 << 7,
    
    /// <summary>
    /// Value objects that are generated implement the generic `: IVogen` interface.
    /// </summary>
    InstancesHaveInterfaceDefinition = 1 << 7,

    /// <summary>
    /// Omit the generation of the generic IVogen interface. Use this if you're declaring it in another
    /// project (either Vogen is generating it, or you've declared it yourself).
    /// </summary>
    OmitInterfaceDeclaration = 1 << 8,

    /// <summary>
    /// The most common usage; generates the definition, containing equals, explicit casts, and factory methods, and
    /// make value objects derive from it.
    /// </summary>
    MostCommon = InstancesHaveInterfaceDefinition |
                 EqualsOperators |
                 ExplicitCastFromPrimitive |
                 ExplicitCastToPrimitive |
                 FactoryMethods
}