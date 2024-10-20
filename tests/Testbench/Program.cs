using System;
using Vogen;


public class Program
{
    public static void Main()
    {
        CustomerId cid1 = CustomerId.From(1);
        SupplierId sid1 = SupplierId.From(1);
        
        Console.WriteLine(cid1 == 1);
        Console.WriteLine(cid1.Equals(1));
        Console.WriteLine(1 == cid1);
        Console.WriteLine(1.Equals(cid1));
        Console.WriteLine(cid1.Equals(1));
        Console.WriteLine(cid1 == CustomerId.From(1));
        Console.WriteLine(CustomerId.From(1) == cid1);
        
        Console.WriteLine(cid1.Equals(sid1));
    }

    static void somethingelse()
    {
        
    }
}

namespace Vogen
{
    public class __ProduceDiagnostics;
}