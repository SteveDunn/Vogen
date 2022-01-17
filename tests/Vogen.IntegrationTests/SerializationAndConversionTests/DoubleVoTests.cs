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
using Vogen.IntegrationTests.SerializationAndConversionTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(double))]
    public partial struct AnotherDoubleVo { }

    public class DoubleVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            DoubleVo.From(18).Equals(DoubleVo.From(18)).Should().BeTrue();
            (DoubleVo.From(18) == DoubleVo.From(18)).Should().BeTrue();

            (DoubleVo.From(18) != DoubleVo.From(19)).Should().BeTrue();
            (DoubleVo.From(18) == DoubleVo.From(19)).Should().BeFalse();

            DoubleVo.From(18).Equals(DoubleVo.From(18)).Should().BeTrue();
            (DoubleVo.From(18) == DoubleVo.From(18)).Should().BeTrue();

            var original = DoubleVo.From(18);
            var other = DoubleVo.From(18);

            ((original as IEquatable<DoubleVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<DoubleVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            DoubleVo.From(18).Equals(AnotherDoubleVo.From(18)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToLong_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonDoubleVo.From(123D);

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedLong = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedLong);
        }

        [Fact]
        public void CanSerializeToLong_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonDoubleVo.From(123D);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedLong = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedLong).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
        {
            var value = 123D;
            var foo = NewtonsoftJsonDoubleVo.From(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDoubleVo>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithSystemTextJsonProvider()
        {
            var value = 123D;
            var foo = SystemTextJsonDoubleVo.From(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDoubleVo>(serializedLong);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToLong_WithBothJsonConverters()
        {
            var foo = BothJsonDoubleVo.From(123D);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedLong1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedLong2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedLong1);
            Assert.Equal(serializedFoo2, serializedLong2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonDoubleVo.From(123D);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonDoubleVo.From(123D);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterDoubleVo.From(123D);

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

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

            var original = new TestEntity { Id = EfCoreDoubleVo.From(123D) };
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

            IEnumerable<DapperDoubleVo> results = await connection.QueryAsync<DapperDoubleVo>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(DapperDoubleVo.From(123D), value);
        }

        [Theory]
        [InlineData(123D)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDoubleVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDoubleVo>(id);
            Assert.Equal(NoJsonDoubleVo.From(123D), id);

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
                             .HasConversion(new EfCoreDoubleVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class TestEntity
        {
            public EfCoreDoubleVo Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonDoubleVo? Id { get; set; }
        }
    }
}