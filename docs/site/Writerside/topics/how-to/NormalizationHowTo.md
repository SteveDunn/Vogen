# Normalize input values

<note>
This topic is incomplete and is currently being improved.
</note>

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
```
There are various compiler errors associated with malformed normalization methods.

This functionality was requested in [this feature request](https://github.com/SteveDunn/Vogen/issues/80).

## Normalize the string before parsing

`NormalizeInput` runs after the underlying value has been produced, so it cannot help when
`Parse` / `TryParse` rejects the input before it gets there. A value object whose string form
carries a prefix — `"E12"` for a value of `12`, for example — fails in the underlying type's
parser long before `NormalizeInput` is called.

For those types, add a private method named `NormalizeParsed`. It takes the string that is about
to be parsed and returns the string that the underlying type's `Parse` / `TryParse` should
receive instead:

```c#
[ValueObject<int>]
public partial struct ErrorCode
{
    private static string NormalizeParsed(string inputToParse) => inputToParse.TrimStart('E');

    public override string ToString() => $"E{Value}";
}
```

`ErrorCode.Parse("E12").Value` is now `12`, and `ErrorCode.TryParse("E12", out var code)` returns
`true` with `code.Value` equal to `12`.

`NormalizeParsed` must be `static`, take a `string`, and return a `string`. It is applied to the
string-based `Parse` and `TryParse` overloads; span-based overloads are left untouched because
they do not take a `string`.

This functionality was requested in [this feature request](https://github.com/SteveDunn/Vogen/issues/969).

