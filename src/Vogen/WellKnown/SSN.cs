using System.Text.RegularExpressions;

namespace Vogen.WellKnown
{
    [ValueObject(typeof(string))]
    public partial struct SSN
    {
        private static readonly Regex validator =
            new(@"^\d{3}-\d{2}\d{4}$", RegexOptions.Compiled);

        public static bool CanParse(string value) =>
            validator.IsMatch(value);

        private static Validation Validate(string value) => CanParse(value) ?
            Validation.Ok :
            Validation.Invalid($"A Social Security Number must match the following regular expression: {validator}");
    }
}
