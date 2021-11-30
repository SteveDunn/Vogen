namespace Vogen.SharedTypes
{
    public class Validation
    {
        public string ErrorMessage { get; }

        public static readonly Validation Ok = new Validation(string.Empty);

        private Validation(string reason) => ErrorMessage = reason;

        public static Validation Invalid(string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                return new Validation("[none provided]");
            }

            return new Validation(reason);
        }
    }
}