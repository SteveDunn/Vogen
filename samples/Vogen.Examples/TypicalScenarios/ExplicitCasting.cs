using System;
using System.Threading.Tasks;

namespace Vogen.Examples.ExplcitCasting
{
    internal class ExplicitCastingScenario : IScenario
    {
        public Task Run()
        {
            // We can create an instance with an explicit cast. If there is validation, it is still run.
            Score score1 = (Score) 20;
            Score score2 = Score.From(20);
            
            Console.WriteLine(score1 == score2); // true
            
            // We can cast an instance to the underlying type too
            int score3 = (int) score2;
            Console.WriteLine(score3 == score2); // true

            return Task.CompletedTask;
        }
    }

    // defaults to int
    [ValueObject]
    internal readonly partial struct Score
    {
    }
}