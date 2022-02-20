using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws: typeof(Whatever.MyValidationException))]

namespace Whatever;



public class Program
{
    public static void Main()
    {
        // throw new MyValidationException("aa");
    }
}


[ValueObject(underlyingType:typeof(float))]
public partial struct CustomerId
{
    private static Validation Validate(float value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}