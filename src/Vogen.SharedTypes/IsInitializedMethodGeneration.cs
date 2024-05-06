using System.Diagnostics;

namespace Vogen;

public enum IsInitializedMethodGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    Omit = 0,
    Generate = 1
}