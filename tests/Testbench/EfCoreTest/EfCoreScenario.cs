using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Vogen.EfCoreTest;

public static class EfCoreScenario
{
    public static void Run()
    {
        using var context = new MyContext();
        //context.Database.Migrate();

        AddAndSaveItems(amount: 10);
        AddAndSaveItems(amount: 10);

        PrintItems();

        FilterItems();

        return;

        static void AddAndSaveItems(int amount)
        {
            using var context = new MyContext();

            for (int i = 0; i < amount; i++)
            {
                var entity = new EmployeeEntity
                {
                    Name = Name.From("Fred #" + i),
                    Age = Age.From(42 + i),
                    Department = Department.From("Quarry"),
                    HireDate = HireDate.From(new DateOnly(1066, 12, 13))
                };

                context.Entities.Add(entity);
            }

            context.SaveChanges();
        }

        static void PrintItems()
        {
            using var ctx = new MyContext();

            var entities = ctx.Entities.ToList();
            Console.WriteLine(string.Join(Environment.NewLine, entities.Select(e => $"ID: {e.Id.Value}, Name: {e.Name}, Age: {e.Age}")));
        }

        static void FilterItems()
        {
            Console.WriteLine();
            Console.WriteLine("FILTERING ITEMS...");
            using var ctx = new MyContext();

            // int age = 50;
            // var entities = from e in ctx.Entities where e != null && e.Age == age select e;
            //Console.WriteLine(string.Join(Environment.NewLine, entities.Select(e => $"ID: {e.Id.Value}, Name: {e.Name}, Age: {e.TheAge}")));

        }
    }

}
