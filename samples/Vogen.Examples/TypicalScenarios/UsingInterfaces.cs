using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vogen.Examples.TypicalScenarios.UsingInterfaces
{
    /// <summary>
    /// Scenario: I want my IDs to have a common interface so that I can pass them to a method that
    /// just wants to know they are an ID.
    /// </summary>
    internal class UsingInterfaces : IScenario
    {
        public Task Run()
        {
            ProcessIds(CustomerId.From(123), AccountId.From(321), DerivedId1.From(666), DerivedId2.From(42));
            return Task.CompletedTask;
        }

        static void ProcessIds(params IHaveAnId<int>[] ids)
        {
            Console.WriteLine("IDs are " + string.Join(", ", ids.Select(i => i.Value)));
        }
    }

    public interface IHaveAnId<out T>
    {
        public T Value { get; }
    }

    // defaults to int
    [ValueObject<int>]
    internal readonly partial struct CustomerId : IHaveAnId<int>
    {
    }

    [ValueObject<int>]
    internal partial struct AccountId : IHaveAnId<int>
    {
    }
    
    // You could derive from this, but if the type you're wrapping is a reference type,
    // then be aware that there could be severe overhead of wrapping a reference type
    // as a value type. One of the  goals of Vogen is to not add too much overhead
    // (in terms of memory/speed) over using the primitive type itself.
    [ValueObject<int>]
    internal partial class Id : IHaveAnId<int> { }
    
    internal class DerivedId1 : Id { }
    internal class DerivedId2 : Id { }
}