using System;
using Vogen.SharedTypes;
// ReSharper disable UnusedMember.Global

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

    public class VendorInformation
    {
        public VendorId VendorId { get; } = VendorId.Unspecified;
    }

    internal static class RepresentingUnspecified
    {
        private static VendorId _unspecified = VendorId.Unspecified;
        
        public static void Run()
        {
            VendorInformation vi = new VendorInformation();
            Console.WriteLine(vi.VendorId == VendorId.Unspecified);
            Console.WriteLine(vi.VendorId != VendorId.Invalid);
        }
    }
}