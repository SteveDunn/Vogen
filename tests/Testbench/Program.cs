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

[ValueObject<decimal>]
public partial struct MyDecimal;

[ValueObject<Guid>]
public partial struct MyGuid;


#nullable disable
[ValueObject]
public partial class MyVo;
#nullable restore

public static class Program
{
    public static void Main()
    {
        MyGuid g = MyGuid.From(Guid.NewGuid());
        Console.WriteLine($"{g:X}");
        g.ToString("X");
        
        var primitive = DateTimeOffset.Now;
        var wrapper = DateOfBirth.From(primitive);

        Console.WriteLine(primitive.ToString(""));
        Console.WriteLine(wrapper.ToString(""));






        Console.WriteLine($"{primitive:o}");
        Console.WriteLine($"{wrapper:G}");




        Console.WriteLine($"{primitive}");
        Console.WriteLine($"{wrapper}");
        
        Console.WriteLine(primitive.ToString("o"));
        Console.WriteLine(wrapper.ToString("o"));
        

        var myd = MyDecimal.From(123.45m);
        Console.WriteLine(string.Format("{0:C}", 123.45m));
        Console.WriteLine(string.Format("{0:C}", myd));
    }
}
