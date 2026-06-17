using System.Diagnostics;
// ReSharper disable UnusedMember.Global

namespace Vogen;

public enum StringComparisonDefault
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    Omit = 0,
    Ordinal = 1,
    OrdinalIgnoreCase = 2,
    CurrentCulture = 3,
    CurrentCultureIgnoreCase = 4,
    InvariantCulture = 5,
    InvariantCultureIgnoreCase = 6,
}
