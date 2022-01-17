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
// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable ConvertToLocalFunction

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(byte))]
    public partial struct AnotherByteVo { }

    public class ByteVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            ByteVo.From(18).Equals(ByteVo.From(18)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(18)).Should().BeTrue();

            (ByteVo.From(18) != ByteVo.From(19)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(19)).Should().BeFalse();

            ByteVo.From(18).Equals(ByteVo.From(18)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(18)).Should().BeTrue();

            var original = ByteVo.From(18);
            var other = ByteVo.From(18);

            ((original as IEquatable<ByteVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<ByteVo>).Equals(original)).Should().BeTrue();
        }


        [Fact]
        public void equality_between_different_value_objects()
        {
            ByteVo.From(18).Equals(AnotherByteVo.From(18)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToShort_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonByteVo.From(123);

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedShort = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedShort);
        }

        [Fact]
        public void CanSerializeToShort_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonByteVo.From(123);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedShort = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedShort).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
        {
            byte value = 123;
            var foo = NewtonsoftJsonByteVo.From(value);
            var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonByteVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromShort_WithSystemTextJsonProvider()
        {
            byte value = 123;
            var foo = SystemTextJsonByteVo.From(value);
            var serializedShort = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonByteVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToShort_WithBothJsonConverters()
        {
            var foo = BothJsonByteVo.From(123);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedShort1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedShort2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedShort1);
            Assert.Equal(serializedFoo2, serializedShort2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonByteVo.From(123);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonByteVo.From(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterByteVo.From(123);

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

            var original = new TestEntity { Id = EfCoreByteVo.From(123) };
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

            IEnumerable<DapperByteVo> results = await connection.QueryAsync<DapperByteVo>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(DapperByteVo.From(123), value);
        }

        [Theory]
        [InlineData((byte) 123)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonByteVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonByteVo>(id);
            Assert.Equal(NoJsonByteVo.From(123), id);

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
                            .HasConversion(new EfCoreByteVo.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreByteVo Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonByteVo? Id { get; set; }
        }
    }
}