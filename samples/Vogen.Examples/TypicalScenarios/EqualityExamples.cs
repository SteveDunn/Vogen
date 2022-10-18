using System;
using System.Threading.Tasks;
// ReSharper disable RedundantCast

namespace Vogen.Examples.TypicalScenarios.Equality
{
    internal class EqualityExamples : IScenario
    {
        public Task Run()
        {
            Console.WriteLine((bool) (Age.From(1) == Age.From(1))); // true
            Console.WriteLine(Age.From(1) == 1); // true
            Console.WriteLine(1 == Age.From(1)); // true

            Console.WriteLine((bool) (Age.From(1) != Age.From(2))); // true
            Console.WriteLine(Age.From(1) != 2); // true
            Console.WriteLine(2 != Age.From(1)); // true

            Console.WriteLine((bool) Age.From(1).Equals(Age.From(1))); // true

            // error CS0019: Operator '==' cannot be applied to operands of type 'Age' and 'Centigrade'
            // Console.WriteLine(Age.From(1) == Centigrade.From(1)); // true

            return Task.CompletedTask;
        }
    }

    [ValueObject]
    public readonly partial struct Age {}

    [ValueObject<float>]
    public readonly partial struct Centigrade { }
}