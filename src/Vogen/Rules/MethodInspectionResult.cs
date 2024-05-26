using System;

namespace Vogen.Rules;

[Flags]
internal enum MethodInspectionResult
{
    Pristine = 0,
    Missing = 1 << 0,
    NotStatic = 1 << 2,
    WrongInputType = 1 << 3
}