using System.ComponentModel;

namespace Vogen;

public enum ParsableForStrings
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unspecified = -1,

    GenerateNothing = 0,
    GenerateMethods = 1,
    GenerateMethodsAndInterface = 2
}

public enum ParsableForPrimitives
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unspecified = -1,

    GenerateNothing = 0,
    HoistMethods = 1,
    HoistMethodsAndInterfaces = 2
}