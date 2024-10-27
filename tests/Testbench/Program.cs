using System;
using System.Globalization;
using System.Runtime.Serialization;
using Vogen;

[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
    conversions: Conversions.EfCoreValueConverter)]

namespace Testbench;

[ValueObject<DateTimeOffset>]
public partial struct DateOfBirth;

// [ValueObject<DateTimeOffset>]
// public partial struct DateOfBirth : IFormatProvider, ICustomFormatter
// {
//     public object? GetFormat(Type? formatType)
//     {
//         if (formatType == typeof(DateOfBirth)) return DateTimeFormatInfo.GetInstance(null);
//         if (formatType == typeof(DateTimeOffset)) return DateTimeFormatInfo.GetInstance(null);
//         return null;
//     }
//
//     public string Format(string? format, object? arg, IFormatProvider? formatProvider)
//     {
//         return "";
//     }
// }

[ValueObject<Guid>]
public partial struct MyGuid;

[ValueObject<decimal>]
public partial struct MyDecimal;


#nullable disable
[ValueObject]
public partial class MyVo;
#nullable restore

public static class Program
{
    public static void Main()
    {
        decimal d = 1.23m;
        
        MyDecimal m = MyDecimal.From(1.23m);
        Console.WriteLine($"{m:000.00}");
        
        Span<char> s = stackalloc char[10];
        d.TryFormat(s, out int written, "000.00", CultureInfo.InvariantCulture);
        Console.WriteLine(s.ToString());
        Console.WriteLine(written.ToString());
    }
}
