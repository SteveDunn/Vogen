using System;
using System.Threading.Tasks;
// ReSharper disable RedundantCast

namespace Vogen.Examples.TypicalScenarios.Instances
{

    /*
     * Instances allow us to create specific static readonly instances of this type.
     */

    internal class InstanceExamples : IScenario
    {
        public Task Run()
        {
            VendorInformation vi = new VendorInformation();
            Console.WriteLine((bool) (vi.VendorId == VendorId.Unspecified)); // true
            Console.WriteLine((bool) (vi.VendorId != VendorId.Invalid)); // true

            // from a text file that is screwed, we'll end up with:
            var invalidVi = VendorInformation.FromTextFile();

            Console.WriteLine((bool) (invalidVi.VendorId == VendorId.Invalid)); // true

            return Task.CompletedTask;
        }
    }

    [ValueObject<float>]
    [Instance("Freezing", 0.0f)]
    [Instance("Boiling", 100.0f)]
    [Instance("AbsoluteZero", -273.15f)]
    public readonly partial struct Centigrade
    {
        public static Validation Validate(float value) =>
            value >= AbsoluteZero.Value ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
    }

    [ValueObject<float>]
    public readonly partial struct Fahrenheit
    {
        public static readonly Fahrenheit Freezing = new(32);
        public static Fahrenheit Boiling { get; } = new(212);
        public static Fahrenheit AbsoluteZero { get; } = new(-459.67f);

        private static Validation Validate(float value) =>
            value >= AbsoluteZero.Value ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
    }

    /*
     * Instances are the only way to avoid validation, so we can create instances
     * that nobody else can. This is useful for creating special instances
     * that represent concepts such as 'invalid' and 'unspecified'.
     */
    [ValueObject]
    [Instance("Unspecified", -1)]
    [Instance("Invalid", -2)]
    public readonly partial struct Age
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    [ValueObject]
    [Instance("Unspecified", 0)]
    [Instance("Invalid", -1)]
    public partial class VendorId
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    [ValueObject<string>]
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
            if (id == VendorId.Unspecified) 
                throw new InvalidOperationException("The vendor ID was unspecified");

            // throw if invalid
            if (id == VendorId.Invalid) 
                throw new InvalidOperationException("The vendor ID was invalid");
            
            // or record it as invalid
            if (id == VendorId.Invalid) return VendorName.Invalid;

            return VendorName.From("abc");
        }
    }
}