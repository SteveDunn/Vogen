using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vogen.Examples.SerializationAndConversion.EFCore;
using Vogen.Examples.Types;

namespace Vogen.Examples.SerializationAndConversion.EFCore
{
    [UsedImplicitly]
    public class EfCoreExamples : IScenario
    {
        public Task Run()
        {
            EfCoreValueConverterUsesValueConverter();
            return Task.CompletedTask;
        }

        private static void EfCoreValueConverterUsesValueConverter()
        {
            AddAndSave(10);
            AddAndSave(10);

            PrintItems();

            static void AddAndSave(int amount)
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

            static void PrintItems()
            {
                using var ctx = new SomeDbContext();

                var entities = ctx.SomeEntities.ToList();
                Console.WriteLine(string.Join(Environment.NewLine, entities.Select(e => $"{e.Id.Value} {e.Name} {e.Age}")));

                Console.WriteLine("Done");
            }
        }
    }
}