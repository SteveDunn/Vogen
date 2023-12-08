# Clean up values

This was requested in [this feature request](https://github.com/SteveDunn/Vogen/issues/80).

By adding a private method named `NormalizeInput`, your type gets a change to, er, normalize input.

The method is given your underlying type, and it returns your underlying type (whether it's the same instance, of a different one).

The example below trims the input string:

```c#
using System;
using System.Threading.Tasks;

namespace Vogen.Examples.TypicalScenarios
{
    // Represent a string scraped from some other text, e.g. a web-page, online article, etc.
    // It cannot be empty, or start / end with whitespace.
    // We have a normalization method that first normalizes the string, then the
    // validation method that validates it.
    [ValueObject(typeof(string))]
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
```
There are various compiler errors associated with malformed normalization methods.