using System;
using System.Threading.Tasks;

namespace Vogen.Examples.TypicalScenarios
{
    internal class BasicExamples : IScenario
    {
        public Task Run()
        {
            // we can't mess up the order of parameters - doing the following results in:
            //      Argument 1: cannot convert from 'SupplierId' to 'CustomerId'
            // new CustomerProcessor().Process(SupplierId.From(123), SupplierId.From(321), Amount.From(123));

            new CustomerProcessor().Process(CustomerId.From(123), SupplierId.From(321), Amount.From(123));

            return Task.CompletedTask;
        }
    }

    // defaults to int
    [ValueObject]
    internal readonly partial struct Score
    {
    }

    // can be internal structs
    [ValueObject]
    internal partial struct Centimeter
    {
    }

    // can be internal classes
    [ValueObject]
    internal partial class Meter
    {
    }

    // can be readonly internal 
    [ValueObject]
    internal readonly partial struct Furlong
    {
    }

    // can be internal sealed
    [ValueObject]
    internal sealed partial class Lumens
    {
    }

    [ValueObject]
    public partial class CustomerId
    {
    }

    [ValueObject]
    public partial class SupplierId
    {
    }

    // defaults to int, but configured to throw an AmountException
    [ValueObject(throws: typeof(AmountException))]
    public partial class Amount
    {
        private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid("Must be > 0"); // throws an AmountException
    }

    public class AmountException : Exception
    {
        public AmountException(string message) : base(message)
        {
        }
    }

    internal class CustomerProcessor
    {
        internal void Process(CustomerId customerId, SupplierId supplierId, Amount amount) =>
            Console.WriteLine($"Processing customer {customerId}, supplier {supplierId}, with amount {amount}");
    }
}