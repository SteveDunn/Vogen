using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

    [UsedImplicitly]
    internal class ValidationExample : IScenario
    {
        public Task Run()
        {
            string[] names = ["Dave Grohl", "David Beckham", "Fred Flintstone"];

            foreach (string name in names)
            {
                try
                {
                    DaveProcessor.Process(Dave.From(name));
                }
                catch (ValueObjectValidationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return Task.CompletedTask;
        }

        private static class DaveProcessor
        {
            internal static void Process(Dave dave) => Console.WriteLine($"Processing {dave}");
        }
    }
}