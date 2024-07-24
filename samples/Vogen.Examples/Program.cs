using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vogen.Examples
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            var scenarioTypes = typeof(Program).Assembly.GetTypes()
                .Where(t => typeof(IScenario).IsAssignableFrom(t) && t != typeof(IScenario)).ToList();

            foreach (var eachScenarioType in scenarioTypes)
            {
                var instance = (IScenario)Activator.CreateInstance(eachScenarioType)!;
                WriteBanner(instance);
                
                await instance.Run();
            }

            Console.WriteLine("Finished");
        }

        private static void WriteBanner(IScenario scenario)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==========================================================");
            Console.WriteLine($"Running {scenario.GetType().Name}");
            string description = scenario.GetDescription();
            if (!string.IsNullOrEmpty(description))
            {
                Console.Write(description);
                Console.WriteLine("------");

            }
        }
    }
}
