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

    // can be internal structs
    [ValueObject(typeof(int))]
    internal partial struct Centimeter
    {
    }

    // can be internal classes
    [ValueObject(typeof(int))]
    internal partial class Meter
    {
    }

    // can be readonly internal 
    [ValueObject(typeof(int))]
    internal readonly partial struct Furlong
    {
    }

    // can be internal sealed
    [ValueObject(typeof(int))]
    internal sealed partial class Lumens
    {
    }

    [ValueObject(typeof(int))]
    public partial class CustomerId
    {
    }

    [ValueObject(typeof(int))]
    public partial class SupplierId
    {
    }

    [ValueObject(typeof(int))]
    public partial class Amount
    {
    }

    internal class CustomerProcessor
    {
        internal void Process(CustomerId customerId, SupplierId supplierId, Amount amount) =>
            Console.WriteLine($"Processing customer {customerId}, supplier {supplierId}, with amount {amount}");
    }
}