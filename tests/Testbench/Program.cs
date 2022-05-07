using System;
using Vogen;

//[assembly: VogenDefaults(conversions: (Conversions)666)]

namespace Whatever;



public class Program
{
    public static void Main()
    {
    }
}

[ValueObject(underlyingType:typeof(int))]
public partial class NormalizedToMax128WithValidation
{
    private static Validation Validate(int value) => value <= 128 && value >= 0 ? Validation.Ok : Validation.Invalid("xxxx");

    private static Int32 NormalizeInput(int input) => Math.Min(128, input);
}

[ValueObject(underlyingType:typeof(int))]
public sealed partial class @class { }