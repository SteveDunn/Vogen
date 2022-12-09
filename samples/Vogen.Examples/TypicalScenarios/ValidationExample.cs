using System;
using System.Threading.Tasks;

namespace Vogen.Examples.TypicalScenarios.ValidationExamples
{
    [ValueObject<string>]

    public partial class Dave
    {
        private static Validation Validate(string value) =>
            value.StartsWith("dave ", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("david ", StringComparison.OrdinalIgnoreCase)
                ? Validation.Ok
                : Validation.Invalid($"must be a dave or david - {value} is neither.");
    }

    internal class ValidationExample : IScenario
    {
        public Task Run()
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

            return Task.CompletedTask;
        }

        private class DaveProcessor
        {
            internal void Process(Dave dave) => Console.WriteLine($"Processing {dave}");
        }
    }
}