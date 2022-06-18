using System;
using System.Linq;

namespace Vogen;

internal static class EnumExtensions
{
    private static readonly int _maxConversionId = Enum.GetValues(typeof(Conversions)).Cast<int>().Max() * 2;
    private static readonly int _maxCustomizationId = Enum.GetValues(typeof(Customizations)).Cast<int>().Max() * 2;

    public static bool IsValidFlags(this Conversions value) => (int) value >= 0 && (int) value < _maxConversionId;
    public static bool IsValidFlags(this Customizations value) => (int) value >= 0 && (int) value < _maxCustomizationId;
}