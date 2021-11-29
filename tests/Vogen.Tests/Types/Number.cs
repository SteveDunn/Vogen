using Vogen.SharedTypes;

namespace Vogen.Tests.Types;

[ValueObject(typeof(int))]
public readonly partial struct Number
{

}

// /// <summary>
// /// A Value Object that is not supported
// /// </summary>
// [ValueObject(typeof(List<Dave>))]
// public partial class Daves
// {
//     private static Validation Validate(List<Dave> value) => value.Count > 0 ? Validation.Ok : Validation.Invalid("no dave's found");
// }