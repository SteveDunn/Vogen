using System;
using Vogen;

namespace Testbench.ToStringScenario;

[ValueObject<DateOnly>]
public sealed partial class CreationDate
{
    // public override string ToString() => "!";
    // public string ToString(string format) => "!!";
    // public string ToString(string format, IFormatProvider p) => "!!!";
    // public string ToString(IFormatProvider p) => "!!!!";
}

[ValueObject<string>]
public partial record class Name
{
    public sealed override string ToString() => "!!";
}

public record R1
{
    public override string ToString() => "R1!";
}

[ValueObject]
public partial record class CustomerId : R1 
{
    public sealed override string ToString() => string.Empty;
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