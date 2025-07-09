using System;
using System.Linq;

namespace Vogen;

internal static class EnumExtensions
{
    private static readonly int _maxConversion = Enum.GetValues(typeof(Conversions)).Cast<int>().Max() * 2;
    private static readonly int _maxCustomization = Enum.GetValues(typeof(Customizations)).Cast<int>().Max() * 2;
    private static readonly int _maxDeserializationStrictness = Enum.GetValues(typeof(DeserializationStrictness)).Cast<int>().Max() * 2;

    
    public static bool IsValidFlags(this Conversions value) => (int) value >= -1 && (int) value < _maxConversion;
    public static bool IsValidFlags(this Customizations value) => (int) value >= 0 && (int) value < _maxCustomization;
    
    public static bool IsValidFlags(this DeserializationStrictness value) => (int) value >= 0 && (int) value < _maxDeserializationStrictness;
}