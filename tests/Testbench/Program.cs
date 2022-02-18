using System;
using Vogen;

//[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.None, throws: typeof(Whatever.GoodException))]

namespace Whatever;



public class Program
{
    public static void Main()
    {
        throw new GoodException();
    }
}



[ValueObject(throws: typeof(Whatever.GoodException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
}

public class BadException1 { }
public class BadException2 { }
public class GoodException : AnotherThing { }

public class AnotherThing : Exception
{
}

public class NotMentionedException { }