using System.Diagnostics;

namespace Vogen;

public enum CastOperator
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    None = 0,
    Explicit,
    Implicit,
}