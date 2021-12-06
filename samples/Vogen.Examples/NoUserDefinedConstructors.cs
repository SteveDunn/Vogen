using System;
using Vogen;

namespace Vogen.Examples.NoUserDefinedConstructors
{
    /*
        You shouldn't be allowed to use a default constructor as it could bypass
        any validation you might have added.
    */

    [ValueObject(typeof(int))]
    public partial struct CustomerId
    {
        private static Validation Validate(in int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");

        // uncomment - error CS0111: Type 'CustomerId' already defines a member called 'CustomerId' with the same parameter type
        // public CustomerId() { }

        // uncomment - error VOG008: Cannot have user defined constructors, please use the From method for creation.
        // public CustomerId(int value) { }

        // uncomment - error VOG008: Cannot have user defined constructors, please use the From method for creation.
        // public CustomerId(int v1, int v2) : this(v1) { }
    }

    [ValueObject(typeof(int))]
    public partial class VendorId
    {
        private static Validation Validate(in int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");

        // uncomment - error CS0111: Type 'VendorId' already defines a member called 'VendorId' with the same parameter type
        // public VendorId() { }

        // uncomment - error VOG008: Cannot have user defined constructors, please use the From method for creation.
        // public VendorId(int value) { }
        // public VendorId(int v1, int v2) : this(v1) { }
        // public VendorId(int v1, int v2, int v3) : this(v1) { }
        // public VendorId(int v1, int v2, int v3, int v4) : this(v1) { }
    }
}