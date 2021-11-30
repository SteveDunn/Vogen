using System;
using Vogen;

namespace Vogen.Examples
{
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

    internal class BasicExample
    {
        public static void Run()
        {
            // we can't mess up the order of parameters - doing the following results in:
            //      Argument 1: cannot convert from 'SupplierId' to 'CustomerId'
            // new CustomerProcessor().Process(SupplierId.From(123), SupplierId.From(321), Amount.From(123));

            new CustomerProcessor().Process(CustomerId.From(123), SupplierId.From(321), Amount.From(123));
        }
    }
}