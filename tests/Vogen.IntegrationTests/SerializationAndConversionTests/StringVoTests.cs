#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vogen.IntegrationTests.TestTypes;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(string))]
    public partial struct AnotherStringVo { }

    public class StringVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            StringVo.From("hello!").Equals(StringVo.From("hello!")).Should().BeTrue();
            (StringVo.From("hello!") == StringVo.From("hello!")).Should().BeTrue();

            (StringVo.From("hello!") != StringVo.From("world!")).Should().BeTrue();
            (StringVo.From("hello!") == StringVo.From("world!")).Should().BeFalse();

            StringVo.From("hello!").Equals(StringVo.From("hello!")).Should().BeTrue();
            (StringVo.From("hello!") == StringVo.From("hello!")).Should().BeTrue();

            var original = StringVo.From("hello!");
            var other = StringVo.From("hello!");

            ((original as IEquatable<StringVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<StringVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            StringVo.From("hello!").Equals(AnotherStringVo.From("hello!")).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonStringVo.From("foo!");

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonStringVo.From("foo!");

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = "foo!";
            var foo = NewtonsoftJsonStringVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonStringVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = "foo!";
            var foo = SystemTextJsonStringVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonStringVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var foo = BothJsonStringVo.From("foo!");

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
            var foo = NoJsonStringVo.From("foo!");

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonStringVo.From("foo!");

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterStringVo.From("foo!");

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

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

            var original = new TestEntity { Id = EfCoreStringVo.From("foo!") };
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
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            IEnumerable<DapperStringVo> results = await connection.QueryAsync<DapperStringVo>("SELECT 'foo!'");

            var value = Assert.Single(results);
            Assert.Equal(DapperStringVo.From("foo!"), value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonStringVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonStringVo>(id);
            Assert.Equal(NoJsonStringVo.From(value?.ToString()), id);

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
                             .HasConversion(new EfCoreStringVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class TestEntity
        {
            public EfCoreStringVo Id { get; set; }
        }
    }
}