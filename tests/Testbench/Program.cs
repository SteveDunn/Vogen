using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vogen;

Console.WriteLine("!!");

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

// public partial struct Foo
// {
//     private Foo(DateTime value) => Value = value;
//
//     public DateTime Value { get; }
//
//     public static Foo From(DateTime value) => new(value);
// }
//
// class VOTYPESystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<Foo>
// {
//     public override Foo Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//     {
//         var v = System.Text.Json.JsonSerializer.Deserialize<TimeSpan>(ref reader, options);
//
//         return Foo.From(reader.GetDateTime());
//     }
//
//     public override void Write(System.Text.Json.Utf8JsonWriter writer, Foo value, System.Text.Json.JsonSerializerOptions options)
//     {
//         // does decimal/double/float
//         writer.WriteStringValue(value.Value);
//     }
// }

EfCoreCustomerIdInt[] ints = new EfCoreCustomerIdInt[10];
var v = ints[0].Value;



var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity { Id = EfCoreCustomerIdInt.From(123) };
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Entities.Add(original);
                context.SaveChanges();
            }
            using (var context = new TestDbContext(options))
            {
                var all = context.Entities.ToList();
                if (all.Count != 1)
                {
                    throw new Exception("!!");
                }

                var item = all.Single();

                if (original.Id != item.Id) throw new Exception("!!!");
            }

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


[ValueObject(typeof(int), conversions: Conversions.TypeConverter | Conversions.EfCoreValueConverter)]
public partial struct EfCoreInt { }
//
// [ValueObject(typeof(string), conversions: Conversions.TypeConverter | Conversions.SystemTextJson)]
// public partial struct CustomerName { }