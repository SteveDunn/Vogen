using System;

namespace StringlyTyped.Examples
{
    internal class BasicExample
    {
        public static void Run()
        {
            // we can't mess up the order of parameters - doing the following results in:
            //      Argument 1: cannot convert from 'SupplierId' to 'CustomerId'
            // new CustomerProcessor().Process(SupplierId.From(123), SupplierId.From(321), Amount.From(123));

            new CustomerProcessor().Process(CustomerId.From(123), SupplierId.From(321), Amount.From(123));
        }


        internal class CustomerId : ValueObject<int, CustomerId>
        {
        }

        internal class SupplierId : ValueObject<int, SupplierId>
        {
        }

        internal class Amount : ValueObject<int, Amount>
        {
        }

        internal class CustomerProcessor
        {
            internal void Process(CustomerId customerId, SupplierId supplierId, Amount amount) =>
                Console.WriteLine($"Processing customer {customerId}, supplier {supplierId}, with amount {amount}");
        }
    }
}