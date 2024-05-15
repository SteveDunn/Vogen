using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vogen.Examples.SerializationAndConversion.EFCore
{
    /// <summary>
    /// Represents examples of using Entity Framework Core (EF Core).
    /// </summary>
    [UsedImplicitly]
    public class EfCoreExamples : IScenario
    {
        public string GetDescription() => """
                                          Uses value objects in EF Core. 
                                          * It creates DB contexts and adds values to it, and saves
                                          * It then create another context and lists the items

                                          It demonstrates:
                                          * how to use value objects in a model
                                          * how to use value objects as primary keys in a model

                                          """; 
        public Task Run()
        {
            AddAndSaveItems(amount: 10);
            AddAndSaveItems(amount: 10);

            PrintItems();

            return Task.CompletedTask;

            static void AddAndSaveItems(int amount)
            {
                using var context = new DbContext();

                for (int i = 0; i < amount; i++)
                {
                    var entity = new PersonEntity
                    {
                        Name = Name.From("Fred #" + i),
                        Age = Age.From(42 + i)
                    };

                    context.Entities.Add(entity);
                }

                context.SaveChanges();
            }

            static void PrintItems()
            {
                using var ctx = new DbContext();

                var entities = ctx.Entities.ToList();
                Console.WriteLine(string.Join(Environment.NewLine, entities.Select(e => $"ID: {e.Id.Value}, Name: {e.Name}, Age: {e.Age}")));
            }
        }
    }
}