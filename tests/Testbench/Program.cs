using System;
using System.Globalization;
using System.Threading.Tasks;
using Vogen;

namespace Testbench;

public class Program
{
    public static async Task Main()
    {
        await Task.CompletedTask;

        var dt = DateTime.Parse("2022-12-13", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind);

        var x = dt.Ticks;

        var dt2 = new DateTime(10000, System.DateTimeKind.Utc);

    }
}

[ValueObject(typeof(float))]
//[Instance(name: "i2", value: "2.34x")]
public readonly partial struct MyInstance
{
}