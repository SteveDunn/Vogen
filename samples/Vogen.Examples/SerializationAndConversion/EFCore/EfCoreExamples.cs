using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vogen.Examples.SerializationAndConversion.EFCore;
using Vogen.Examples.Types;

namespace Vogen.Examples.SerializationAndConversion.EFCore
{
    public class EfCoreExamples : IScenario
    {
        public Task Run()
        {
            EfCoreValueConverterUsesValueConverter();
            return Task.CompletedTask;
        }

        private void EfCoreValueConverterUsesValueConverter()
        {
                addAndSave(10);
                addAndSave(10);

                printItems();

            static void addAndSave(int amount)
            {
                using var context = new SomeDbContext();

                for (int i = 0; i < amount; i++)
                {
                    var entity = new SomeEntity
                    {
                        Name = Name.From("Fred # " + i),
                        Age = Age.From(42 + i)
                    };

                    context.SomeEntities.Add(entity);
                }

                context.SaveChanges();
            }

            static void printItems()
            {
                using var ctx = new SomeDbContext();

                var entities = ctx.SomeEntities.ToList();
                Console.WriteLine(string.Join(Environment.NewLine, entities.Select(e => $"{e.Id.Value} {e.Name} {e.Age}")));

                Console.WriteLine("Done");
            }
        }
    }
}