using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Testbench.FooTests;
using Vogen;

Console.WriteLine("!!");


Tests.TestNoJson();



// SqlMapper.ResetTypeHandlers();
// SqlMapper.AddTypeHandler(typeof(DapperBoolVo), new DapperDateTimeVo.DapperBoolVo());
//
// using var connection = new SqliteConnection("DataSource=:memory:");
// await connection.OpenAsync();
//
// connection.Execute(
//     @"create table Customer
//                 (
//                     Id                                  integer identity primary key,
//                     IsSpecial                         boolean not null
//                 )");
//
// var c = new Customer
// {
//     Id = 123,
//     IsSpecial = DapperBoolVo.From(true)
// };
//
// connection.Execute("insert into Customer(Id, IsSpecial) values(@Id, @IsSpecial)", c);
// Customer f2 = connection.Query<Customer>("select * from Customer where Id = @Id", c).Single();
//
// class Customer
// {
//     public int Id { get; set; }
//     public DapperBoolVo IsSpecial { get; set; }
// }

public class TestEntity
{
    public EfCoreCustomerIdInt Id { get; set; }
}


public class TestDbContext : DbContext
{
    public DbSet<TestEntity> Entities { get; set; } = null!;

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<TestEntity>(builder =>
            {
                builder
                    .Property(x => x.Id)
                    .HasConversion(new EfCoreCustomerIdInt.EfCoreValueConverter())
                    .ValueGeneratedNever();
            });
    }
}


// [ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.EfCoreValueConverter)]
// public partial struct EfCoreInt { }
//
// [ValueObject(typeof(string), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
// public partial struct CustomerName { }

//[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
public partial struct DapperDateTimeVo { }

// [ValueObject(conversions: Conversions.DapperTypeHandler | Conversions.TypeConverter, underlyingType: typeof(DateTime))]
// public partial struct SteveDataTime { }



[ValueObject(typeof(char), conversions: Conversions.TypeConverter)]
public partial struct MyCharacter { }

//
// [ValueObject(typeof(TimeSpan), conversions: Conversions.TypeConverter)]
// public partial struct Duration { }
//
// [ValueObject(typeof(DateTime), conversions: Conversions.TypeConverter)]
// public partial struct ReminderTime { }
// //
// [ValueObject(typeof(float), conversions: Conversions.TypeConverter)]
// public partial struct Velocity { }
//
// [ValueObject(typeof(double), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
// public partial struct Velocity2 { }
//
// [ValueObject(typeof(decimal), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
// public partial struct Amount { }
//
// [ValueObject(typeof(short), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
// public partial struct ShortAmount { }

// [ValueObject(typeof(TimeSpan))]
// public partial struct Duration
// {
//     private static Validation Validate(TimeSpan timeSpan) =>
//         timeSpan >= TimeSpan.Zero ? Validation.Ok : Validation.Invalid("Cannot be negative");
//
//     public Duration DecreaseBy(TimeSpan amount) => Duration.From(Value - amount);
// }
