// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable ArrangeMethodOrOperatorBody

using System;
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    [ValueObject(typeof(int))]
    [Instance("Unspecified", 0)]
    [Instance("Invalid", -1)]
    public partial class VendorId
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    [ValueObject(typeof(string))]
    [Instance("Invalid", "[INVALID]")]
    public partial class VendorName
    {
    }

    public class VendorInformation
    {
        public VendorId VendorId { get; private init; } = VendorId.Unspecified;

        public static VendorInformation FromTextFile()
        {
            // image the text file is screwed...
            return new VendorInformation
            {
                VendorId = VendorId.Invalid
            };
        }
    }

    public class VendorRelatedThings
    {
        public VendorName GetVendorName(VendorId id)
        {
            if (id == VendorId.Unspecified) throw new InvalidOperationException("The vendor ID was unspecified");

            // throw if invalid
            if (id == VendorId.Invalid) throw new InvalidOperationException("The vendor ID was invalid");
            
            // or record it as invalid
            if (id == VendorId.Invalid) return VendorName.Invalid;

            return VendorName.From("abc");
        }
    }

    internal static class RepresentingUnspecified
    {
        private static VendorId _unspecified = VendorId.Unspecified;
        
        public static void Run()
        {
            VendorInformation vi = new VendorInformation();
            Console.WriteLine(vi.VendorId == VendorId.Unspecified); // true
            Console.WriteLine(vi.VendorId != VendorId.Invalid); // true

            // from a text file that is screwed, we'll end up with
            var invalidVi = VendorInformation.FromTextFile();
            
            Console.WriteLine(invalidVi.VendorId == VendorId.Invalid); // true
        }
    }
}