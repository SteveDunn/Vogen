using System.Diagnostics;

namespace Vogen;

public enum TryFromGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    Omit = 0,
    GenerateBoolMethod = 1,
    GenerateErrorOrMethod = 2,
    GenerateBoolAndErrorOrMethods = 3
}