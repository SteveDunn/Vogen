using System;
using System.Linq;

namespace Vogen;

internal static class EnumExtensions
{
    private static readonly int _maxConversionId = Enum.GetValues(typeof(Conversions)).Cast<int>().Max() * 2;

    public static bool IsSet(this Conversions value, Conversions flag)
        => (value & flag) == flag;

    public static bool IsValidFlags(this Conversions value) => (int) value >= 0 && (int) value < _maxConversionId;
}