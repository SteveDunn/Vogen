using System.Diagnostics;
// ReSharper disable UnusedMember.Global

namespace Vogen;

public enum StringComparersGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    Omit = 0,
    Generate = 1,
}