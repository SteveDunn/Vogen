#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vogen.IntegrationTests.SerializationAndConversionTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
// ReSharper disable EqualExpressionComparison
// ReSharper disable RedundantCast
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable SuspiciousTypeConversion.Global

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(DateTime))]
    public readonly partial struct AnotherDateTimeVo { }

    public class DateTimeVoTests
    {
        private static readonly DateTime _date1 = DateTime.Now;
        private static readonly DateTime _date2 = DateTime.Now.AddMinutes(42.69);
        
        [Fact]
        public void equality_between_same_value_objects()
        {
            DateTimeVo.From(_date1).Equals(DateTimeVo.From(_date1)).Should().BeTrue();
            (DateTimeVo.From(_date1) == DateTimeVo.From(_date1)).Should().BeTrue();

            (DateTimeVo.From(_date1) != DateTimeVo.From(_date2)).Should().BeTrue();
            (DateTimeVo.From(_date1) == DateTimeVo.From(_date2)).Should().BeFalse();

            DateTimeVo.From(_date1).Equals(DateTimeVo.From(_date1)).Should().BeTrue();
            (DateTimeVo.From(_date1) == DateTimeVo.From(_date1)).Should().BeTrue();

            var original = DateTimeVo.From(_date1);
            var other = DateTimeVo.From(_date1);

            ((original as IEquatable<DateTimeVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<DateTimeVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            DateTimeVo.From(_date1).Equals(AnotherDateTimeVo.From(_date1)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var g1 = NewtonsoftJsonDateTimeVo.From(_date1);

            string serialized = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serialized, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonDateTimeVo.From(_date1);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _date1;
            var foo = NewtonsoftJsonDateTimeVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDateTimeVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _date1;
            var foo = SystemTextJsonDateTimeVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDateTimeVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var foo = BothJsonDateTimeVo.From(_date1);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedString2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedString1);
            Assert.Equal(serializedFoo2, serializedString2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonDateTimeVo.From(_date1);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + _date1.ToString("o") + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonDateTimeVo.From(_date1);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{_date1:o}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterDateTimeVo.From(_date1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + _date1.ToString("o") + "\"}";

            Assert.Equal(expected, newtonsoft);
            Assert.Equal(expected, systemText);
        }

        [Fact]
        public void WhenEfCoreValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity { Id = EfCoreDateTimeVo.From(_date1) };
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Entities.Add(original);
                context.SaveChanges();
            }
            using (var context = new TestDbContext(options))
            {
                var all = context.Entities.ToList();
                var retrieved = Assert.Single(all);
                Assert.Equal(original.Id, retrieved.Id);
            }
        }

        [Fact]
        public async Task WhenDapperValueConverterUsesValueConverter()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            IEnumerable<DapperDateTimeVo> results = await connection.QueryAsync<DapperDateTimeVo>("SELECT '2022-01-15 19:08:49.5413764'");

            DapperDateTimeVo actual = Assert.Single(results);

            var expected = DapperDateTimeVo.From(new DateTime(2022,01,15,19,08,49,DateTimeKind.Utc).AddTicks(5413764));
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("2022-01-15T19:08:49.5413764+00:00")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDateTimeVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDateTimeVo>(id);
            Assert.Equal(NoJsonDateTimeVo.From(DateTime.ParseExact(value, "O", CultureInfo.InvariantCulture)), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        public class TestDbContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; }

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
                             .HasConversion(new EfCoreDateTimeVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class TestEntity
        {
            public EfCoreDateTimeVo Id { get; set; }
        }
    }
}