using System;
using System.Threading.Tasks;

namespace Vogen.Examples.TypicalScenarios.Normalization
{
    // Represent a string scraped from some other text, e.g. a web-page, online article, etc.
    // It cannot be empty, or start / end with whitespace.
    // We have a normalization method that first normalizes the string, then the
    // validation method that validates it.
    [ValueObject<string>]
    public partial class ScrapedString
    {
        private static Validation Validate(string value)
        {
            return value.Length == 0 ? Validation.Invalid("Can't be empty") : Validation.Ok;
        }

        private static string NormalizeInput(string input) => input.Trim();
    }

    internal class NormalizationExample : IScenario
    {
        public Task Run()
        {
            /* output:
                Processing "Fred Flintstone"
                Processing "Wilma Flintstone"
                Processing "Barney Rubble"
                Can't be empty
             */
            string[] names = new[] { " Fred Flintstone", "Wilma Flintstone\t", " Barney Rubble  \t", " \t    \t" };

            var processor = new Processor();

            foreach (string name in names)
            {
                try
                {
                    processor.Process(ScrapedString.From(name));
                }
                catch (ValueObjectValidationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return Task.CompletedTask;
        }

        private class Processor
        {
            internal void Process(ScrapedString item) => Console.WriteLine($"Processing \"{item}\"");
        }
    }
}