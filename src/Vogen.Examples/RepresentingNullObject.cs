using Vogen.SharedTypes;
// ReSharper disable UnusedMember.Global

namespace Vogen.Examples
{
    [ValueObject(typeof(int))]
    [Instance("Unspecified", 0)]
    public partial class VendorId
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    internal static class RepresentingUnspecified
    {
        public static void Run()
        {
            _ = VendorId.Unspecified;
        }
    }
}