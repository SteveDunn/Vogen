using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Testbench.FooTests;

public static class Tests
{
    private static Bar _bar1 = new Bar {Age = 42, Name = "Fred"};

    public static void TestEfCore()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity
        {
            BarField = new Bar{Age = 12, Name = "Wilma"},
            FooField = NoJsonFooVo.From(_bar1)
        };
        //var original = new TestEntity { FooField = NoJsonFooVo.From(_bar1) };
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(original);
            context.SaveChanges();
        }
        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            // var retrieved = Assert.Single(all);
            //Assert.Equal(original.FooField, retrieved.FooField);
        }
    }

    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; } = null!;

        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<TestEntity>()
                .Property(x => x.BarField)
                .HasConversion(new BarConverter());

            modelBuilder
                .Entity<TestEntity>()
                .Property(x => x.FooField)
                .HasConversion(new NoJsonFooVo.EfCoreValueConverter());

        }
    }

    public class BarConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Bar, string>
    {
        public BarConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null!)
            : base(
                convertToProviderExpression: vo => System.Text.Json.JsonSerializer.Serialize(vo, default(JsonSerializerOptions)),
                text => System.Text.Json.JsonSerializer.Deserialize<Bar>(text, default(JsonSerializerOptions)),
                mappingHints
            )
        { }
    }



    public class TestEntity
    {
        public int Id { get; set; }

        public Bar BarField { get; set; } = default!;
        
        public NoJsonFooVo FooField { get; set; }

        //public string FooField { get; set; } = "";
    }
}