using System;
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    [ValueObject(typeof(string))]

    public partial class Dave
    {
        private static Validation Validate(string value) =>
            value.StartsWith("dave ", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("david ", StringComparison.OrdinalIgnoreCase)
                ? Validation.Ok
                : Validation.Invalid($"must be a dave or david - {value} is neither.");
    }

    internal static class ValidationExample
    {
        public static void Run()
        {
            string[] names = new[] { "Dave Grohl", "David Beckham", "Fred Flintstone" };

            var processor = new DaveProcessor();

            foreach (string name in names)
            {
                try
                {
                    processor.Process(Dave.From(name));
                }
                catch (ValueObjectValidationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private class DaveProcessor
        {
            internal void Process(Dave dave) => Console.WriteLine($"Processing {dave}");
        }
    }
}