// ReSharper disable UnusedVariable
#pragma warning disable CS0219

namespace Vogen.Examples.NoDefaulting
{
    /*
        You shouldn't be allowed to `default` a Value Object as it bypasses
        any validation you might have added.
    */
    
    public class Naughty
    {
        public Naughty()
        {
            // uncomment for - error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
            // CustomerId c = default; 
            // var c2 = default(CustomerId);

            // VendorId v = default;
            // var v2 = default(VendorId);

            // uncomment for - error VOG010: Type 'VendorId' cannot be constructed with 'new' as it is prohibited.
            // var v3 = new VendorId();

            // uncomment for - error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.
            //var v4 = new CustomerId();
            // CustomerId v5 = new();
            // var _ = new CustomerId();
            // new CustomerId();

            // this is valid syntax as it can't easily be picked up at compile time,
            // but it throws a ValueObjectValidationException at runtime.
            CustomerId[] customerIds = new CustomerId[10];
        }

        // a method can't accept a VO and default it
        // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited
        // public void CallMe(CustomerId customerId = default)
        // {
        //     int _ = customerId.Value;
        // }

        // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
        // public CustomerId GetCustomerId() => default;

        //  error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.
        // public CustomerId GetCustomerId() => new();
        // public CustomerId GetCustomerId() => new CustomerId();
        
        
    }

    [ValueObject]
    public partial struct CustomerId { }

    [ValueObject]
    public partial class VendorId { }
}