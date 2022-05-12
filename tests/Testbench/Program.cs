using System;
using Vogen;

//[assembly: VogenDefaults(conversions: (Conversions)666)]

namespace Whatever;

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
internal sealed partial record class public_partial_classConversions_DapperTypeHandlerDateTime { }



public class Program
{
    public static void Main()
    {
        //var s = Score.From(1);
        //Console.WriteLine(s.Value);
    }
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