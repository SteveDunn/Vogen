using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable AccessToStaticMemberViaDerivedType
// ReSharper disable ClassNeverInstantiated.Global

namespace Vogen.Examples.TypicalScenarios.StaticAbstractsScenario
{
    /// <summary>
    /// Scenario: I want my IDs to have a common interface so that I can pass them to a method that
    /// just wants to know they are an ID.
    /// </summary>
    [UsedImplicitly]
    internal class StaticAbstractsScenario : IScenario
    {
        public string GetDescription()
        {
            return """
                   Uses static abstracts in interfaces to demonstrate how Vogen types can be extended
                   via their type rather than their instance.
                   The scenario used here is a 'factory': it doesn't need to know the type of the value
                   object, it just needs to know that it *is* a value object that wraps an integer
                   primitive.
                   """;
        }

        public Task Run()
        {
            UniqueIdFactory f = new();

            var customer1 = f.Next<CustomerId>();
            var customer2 = f.Next<CustomerId>();
            var account1 = f.Next<AccountId>();
            var account2 = f.Next<AccountId>();
            
            Console.WriteLine($"Customer 1 has ID of {customer1.Value}");
            Console.WriteLine($"Customer 2 has ID of {customer2.Value}");
            Console.WriteLine($"Account 1 has ID of {account1.Value}");
            Console.WriteLine($"Account 2 has ID of {account2.Value}");
            
            
            return Task.CompletedTask;
        }
    }

    public class UniqueIdFactory
    {
        private int _id;
        
        public TVo Next<TVo>() where TVo : IVogen<TVo, int>
        {
            return TVo.From(Interlocked.Increment(ref _id));
        }
    }

    [ValueObject]
    internal readonly partial struct CustomerId
    {
    }

    [ValueObject]
    internal readonly partial struct AccountId
    {
    }
}