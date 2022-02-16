using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vogen;


[assembly: Vogen.VogenDefaults(underlyingType: typeof(System.String), conversions: Vogen.Conversions.None, typeOfValidationException:typeof(MyValidationException))]

var id = MyId.From("123");
Console.WriteLine($"{id.Value} ({id.Value.GetType()})");


// [ValueObject(conversions: Conversions.DapperTypeHandler, validationExceptionType: typeof(Vogen.IntegrationTests.SnapshotTests.MyValidationException))]
// public partial struct CustomerId
// {
//     private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("xxxx");
// }


//[ValueObject(validationExceptionType: typeof(DodgyException))]
[ValueObject]
public partial class MyId
{
    private static Validation Validate(string value) => value.Length > 0 ? Validation.Ok : Validation.Invalid("xxxx");
}

public class DodgyException : Exception
{
    
}


[Serializable]
public class MyValidationException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public MyValidationException() { }
    public MyValidationException(string message) : base(message) { }
    public MyValidationException(string message, Exception inner) : base(message, inner) { }

    protected MyValidationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}