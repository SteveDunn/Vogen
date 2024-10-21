using System;
using Vogen;

namespace Testbench.ToStringScenario;

[ValueObject<DateOnly>]
public partial struct CreationDate
{
    // public override string ToString() => "!";
    // public string ToString(string format) => "!!";
    // public string ToString(string format, IFormatProvider p) => "!!!";
    // public string ToString(IFormatProvider p) => "!!!!";
}


public static class Runner
{
    public static void Run()
    {
        DateOnly primitive = new DateOnly(2020, 1, 1);
        var wrapper = CreationDate.From(primitive);
        Console.WriteLine("No parameters:");
        Console.WriteLine(primitive.ToString());
        Console.WriteLine(wrapper.ToString());

        Console.WriteLine();
        Console.WriteLine("iso parameter:");
        Console.WriteLine(primitive.ToString("o"));
        Console.WriteLine(wrapper.ToString("o"));
    }
}