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
    [ValueObject(underlyingType: typeof(float))]
    public partial struct AnotherFloatVo { }

    public class FloatVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            FloatVo.From(18).Equals(FloatVo.From(18)).Should().BeTrue();
            (FloatVo.From(18) == FloatVo.From(18)).Should().BeTrue();

            (FloatVo.From(18) != FloatVo.From(19)).Should().BeTrue();
            (FloatVo.From(18) == FloatVo.From(19)).Should().BeFalse();

            FloatVo.From(18).Equals(FloatVo.From(18)).Should().BeTrue();
            (FloatVo.From(18) == FloatVo.From(18)).Should().BeTrue();

            var original = FloatVo.From(18);
            var other = FloatVo.From(18);

            ((original as IEquatable<FloatVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<FloatVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            FloatVo.From(18).Equals(AnotherFloatVo.From(18)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToInt_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonFloatVo.From(123);

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedInt = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedInt);
        }

        [Fact]
        public void CanSerializeToInt_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonFloatVo.From(123);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedInt = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedInt).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromInt_WithNewtonsoftJsonProvider()
        {
            var value = 123;
            var foo = NewtonsoftJsonFloatVo.From(value);
            var serializedInt = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFloatVo>(serializedInt);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromInt_WithSystemTextJsonProvider()
        {
            var value = 123;
            var foo = SystemTextJsonFloatVo.From(value);
            var serializedInt = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonFloatVo>(serializedInt);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToInt_WithBothJsonConverters()
        {
            var foo = BothJsonFloatVo.From(123);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedInt1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedInt2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedInt1);
            Assert.Equal(serializedFoo2, serializedInt2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonFloatVo.From(123);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":" + foo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonFloatVo.From(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterFloatVo.From(123);

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

            var original = new TestEntity { Id = EfCoreFloatVo.From(123) };
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

            IEnumerable<DapperFloatVo> results = await connection.QueryAsync<DapperFloatVo>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(DapperFloatVo.From(123), value);
        }

        [Theory]
        [InlineData((float)123)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonFloatVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonFloatVo>(id);
            Assert.Equal(NoJsonFloatVo.From(123), id);

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
                             .HasConversion(new EfCoreFloatVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class TestEntity
        {
            public EfCoreFloatVo Id { get; set; }
        }
    }
}