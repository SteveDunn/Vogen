using System;
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

[Flags]
public enum ParsableGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = 1 << 8,
    HoistParseAndTryParseMethodsFromPrimitive = 1 << 0,
    HoistInterfacesFromPrimitive = 1 << 1,
    GenerateIParsableWherePrimitiveIsAString = 1 << 2
}