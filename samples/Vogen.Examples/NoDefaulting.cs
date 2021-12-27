using System;
using Vogen;
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
        }

        // public void CallMe(CustomerId customerId = default)
        // {
        //     int _ = customerId.Value;
        // }
    }

    [ValueObject(typeof(int))]
    public partial struct CustomerId { }

    [ValueObject(typeof(int))]
    public partial class VendorId { }
}