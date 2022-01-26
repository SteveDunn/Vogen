using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vogen.Examples
{
    class Program
    {
        static Task Main(string[] args)
        {
            var scenarioTypes = typeof(Program).Assembly.GetTypes().Where(t => typeof(IScenario).IsAssignableFrom(t) && t != typeof(IScenario)).ToList();

            foreach (var eachScenarioType in scenarioTypes)
            {
                ((IScenario)Activator.CreateInstance(eachScenarioType)).Run();
            }

            Console.WriteLine("Finished");
            
            return Task.CompletedTask;
        }
    }
}
