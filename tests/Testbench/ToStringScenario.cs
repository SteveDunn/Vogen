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

[ValueObject]
public readonly partial struct Whatever1;

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

        Whatever1 w1 = Whatever1.From(42);

        var r = new ROS();
        r.DoSomething(w1);

        var h = new Hash();
        h.ToString();
        FileHash.From(h).ToString();
    }
}

public class Hash
{
    // /// <summary>
    // /// Hash special to string!!
    // /// </summary>
    // /// <returns>aaaa</returns>
    // public override string ToString() => base.ToString()!;
}

[ValueObject<Hash>]
public readonly partial struct FileHash
{
    // /// <inheritdoc cref="Testbench.ToStringScenario.Hash.ToString()"/>
    // // /// <returns>dd <inheritdoc cref="Testbench.ToStringScenario.Hash.ToString()"  select="returns"/></returns>
    //
    // public override string ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";
}

// /// <inheritdoc cref="Testbench.ToStringScenario.Hash.ToString()"/>
// /// <returns>bbbb
// /// <inheritdoc select="returns"/>
// /// </returns>


public readonly struct ROS
{
    public readonly void DoSomething(in Whatever1 w)
    {
        Console.WriteLine(w.ToString());
        
    }
}



                   
