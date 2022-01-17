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
    [ValueObject(underlyingType: typeof(char))]
    public partial struct AnotherCharVo { }

    public class CharVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            CharVo.From('a').Equals(CharVo.From('a')).Should().BeTrue();
            (CharVo.From('a') == CharVo.From('a')).Should().BeTrue();

            (CharVo.From('a') != CharVo.From('b')).Should().BeTrue();
            (CharVo.From('a') == CharVo.From('b')).Should().BeFalse();

            CharVo.From('a').Equals(CharVo.From('a')).Should().BeTrue();
            (CharVo.From('a') == CharVo.From('a')).Should().BeTrue();

            var original = CharVo.From('a');
            var other = CharVo.From('a');

            ((original as IEquatable<CharVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<CharVo>).Equals(original)).Should().BeTrue();
        }


        [Fact]
        public void equality_between_different_value_objects()
        {
            CharVo.From('a').Equals(AnotherCharVo.From('a')).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToShort_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonCharVo.From('a');

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedShort = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedShort);
        }

        [Fact]
        public void CanSerializeToShort_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonCharVo.From('a');

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedShort = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedShort).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
        {
            char value = 'a';
            var foo = NewtonsoftJsonCharVo.From(value);
            var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonCharVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromShort_WithSystemTextJsonProvider()
        {
            char value = 'a';
            var foo = SystemTextJsonCharVo.From(value);
            var serializedShort = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonCharVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToShort_WithBothJsonConverters()
        {
            var foo = BothJsonCharVo.From('a');

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
            var foo = NoJsonCharVo.From('a');

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"a\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonCharVo.From('a');

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterCharVo.From('a');

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"a\"}";

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

            var original = new TestEntity { Id = EfCoreCharVo.From('a') };
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

            IEnumerable<DapperCharVo> results = await connection.QueryAsync<DapperCharVo>("SELECT 'a'");

            var value = Assert.Single(results);
            Assert.Equal(DapperCharVo.From('a'), value);
        }

        [Theory]
        [InlineData((char) 97)]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonCharVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonCharVo>(id);
            Assert.Equal(NoJsonCharVo.From('a'), id);

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
                            .HasConversion(new EfCoreCharVo.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreCharVo Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonCharVo? Id { get; set; }
        }
    }
}