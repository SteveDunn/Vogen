namespace StringlyTyped.Examples
{
    internal static class RepresentingNullObject
    {
        public static void Run()
        {
            var id = CustomerId.NullCustomerId;
        }

        class CustomerId : ValueObject<int, CustomerId>
        {
            public override Validation Validate()
            {
                return Value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
            }

            public static readonly CustomerId NullCustomerId = CustomerId.From(0);
        }
    }
}