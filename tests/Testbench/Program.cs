using System;
using Vogen;

//[assembly: VogenDefaults(conversions: (Conversions)666)]

namespace Whatever;

[ValueObject]
[Instance("Min", 10, @"<abc>whatevs</abc>")]
internal sealed partial record MyVo
{
    /// <summary>noxml</summary>
    /// <returns>An immutable shared instance of "T:Whatever.MyVo"</returns>
    public static int Foo = 123;
}

public class Program
{
    public static void Main()
    {
    }
}

[ValueObject]
public partial record Age
{
}



// [ValueObject(underlyingType:typeof(int))]
// public partial class NormalizedToMax128WithValidation
// {
//     private static Validation Validate(int value) => value <= 128 && value >= 0 ? Validation.Ok : Validation.Invalid("xxxx");
//
//     private static Int32 NormalizeInput(int input) => Math.Min(128, input);
// }

// [ValueObject]
// public readonly partial record struct Score
// {
//     private static Validation Validate(int value) => value >= 0 ? Validation.Ok : Validation.Invalid("Score must be zero or more");
// }