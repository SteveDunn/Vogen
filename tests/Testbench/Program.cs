using System;
using Vogen;

//[assembly: VogenDefaults(conversions: (Conversions)666)]

namespace Whatever;



public class Program
{
    public static void Main()
    {
        Console.WriteLine(CustomerId.From(129).Value);
    }
}

// [ValueObject(typeof(int))]
// public partial struct CustomerId2
// {
//     private static int NormalizeInput(bool value) => 0;
// }


[ValueObject(underlyingType:typeof(int))]
public partial class CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");

    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}