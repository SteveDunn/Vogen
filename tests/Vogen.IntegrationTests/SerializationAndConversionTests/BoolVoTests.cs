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
// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable ConvertToLocalFunction

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(bool))]
    public partial struct AnotherBoolVo { }

    public class BoolVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            BoolVo.From(true).Equals(BoolVo.From(true)).Should().BeTrue();
            (BoolVo.From(false) == BoolVo.From(false)).Should().BeTrue();

            (BoolVo.From(true) != BoolVo.From(false)).Should().BeTrue();
            (BoolVo.From(true) == BoolVo.From(false)).Should().BeFalse();

            BoolVo.From(true).Equals(BoolVo.From(true)).Should().BeTrue();
            (BoolVo.From(true) == BoolVo.From(true)).Should().BeTrue();

            var original = BoolVo.From(true);
            var other = BoolVo.From(true);

            ((original as IEquatable<BoolVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<BoolVo>).Equals(original)).Should().BeTrue();
        }


        [Fact]
        public void equality_between_different_value_objects()
        {
            BoolVo.From(true).Equals(AnotherBoolVo.From(true)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToShort_WithNewtonsoftJsonProvider()
        {
            var foo = NewtonsoftJsonBoolVo.From(true);

            string serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            string serializedBool = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedBool);
        }

        [Fact]
        public void CanSerializeToShort_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonBoolVo.From(true);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedShort = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedShort).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
        {
            bool value = true;
            var foo = NewtonsoftJsonBoolVo.From(value);
            var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonBoolVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromShort_WithSystemTextJsonProvider()
        {
            bool value = true;
            var foo = SystemTextJsonBoolVo.From(value);
            var serializedShort = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonBoolVo>(serializedShort);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToShort_WithBothJsonConverters()
        {
            var foo = BothJsonBoolVo.From(true);

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
            var foo = NoJsonBoolVo.From(true);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":true}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonBoolVo.From(true);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{(bool)foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterBoolVo.From(true);

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":true}";

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

            var original = new TestEntity { Id = EfCoreBoolVo.From(true) };
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

            IEnumerable<DapperBoolVo> results = await connection.QueryAsync<DapperBoolVo>("SELECT true");

            var value = Assert.Single(results);
            Assert.Equal(DapperBoolVo.From(true), value);
        }

        [Theory]
        [InlineData("true", true, "True")]
        [InlineData("True", true, "True")]
        [InlineData("false", false, "False")]
        [InlineData("False", false, "False")]
        public void TypeConverter_CanConvertToAndFrom_strings(object input, bool expectedBool, string expectedString)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(NoJsonBoolVo));
            
            object converted = converter.ConvertFrom(input);
            Assert.IsType<NoJsonBoolVo>(converted);
            Assert.Equal(NoJsonBoolVo.From(expectedBool), converted);

            object reconverted = converter.ConvertTo(converted, input.GetType());
            Assert.Equal(expectedString, reconverted);
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public void TypeConverter_CanConvertToAndFrom_bools(bool input, string expectedString)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(NoJsonBoolVo));

            NoJsonBoolVo v = NoJsonBoolVo.From(input);
            
            object asString = converter.ConvertTo(v, typeof(string));
            Assert.Equal(expectedString, asString);
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
                            .HasConversion(new EfCoreBoolVo.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreBoolVo Id { get; set; }
        }
    }
}