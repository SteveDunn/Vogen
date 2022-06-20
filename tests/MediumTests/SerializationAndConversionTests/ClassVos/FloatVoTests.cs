﻿#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Vogen.IntegrationTests.TestTypes.ClassVos;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos
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
        public void CanSerializeToFloat_WithNewtonsoftJsonProvider()
        {
            var vo = NewtonsoftJsonFloatVo.From(123.45f);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            string serializedFloat = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            Assert.Equal(serializedVo, serializedFloat);
        }

        [Fact]
        public void RoundTrip_WithNsj()
        {
            var vo = NewtonsoftJsonFloatVo.From(123.45f);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFloatVo>(serializedVo)!;

            deserializedVo.Value.Should().Be(123.45f);
        }

        [Fact]
        public void RoundTrip_WithStj()
        {
            var vo = SystemTextJsonFloatVo.From(123.45f);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonFloatVo>(serializedVo)!;

            deserializedVo.Value.Should().Be(123.45f);
        }

        [Fact]
        public void CanSerializeToFloat_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonFloatVo.From(123);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedFloat = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedFloat).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromFloat_WithNewtonsoftJsonProvider()
        {
            var value = 123;
            var vo = NewtonsoftJsonFloatVo.From(value);
            var serializedFloat = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFloatVo>(serializedFloat);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromFloat_WithSystemTextJsonProvider()
        {
            var value = 123;
            var vo = SystemTextJsonFloatVo.From(value);
            var serializedFloat = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonFloatVo>(serializedFloat);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToFloat_WithBothJsonConverters()
        {
            var vo = BothJsonFloatVo.From(123.45f);

            var serializedVo1 = NewtonsoftJsonSerializer.SerializeObject(vo);
            var serializedFloat1 = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            var serializedVo2 = SystemTextJsonSerializer.Serialize(vo);
            var serializedFloat2 = SystemTextJsonSerializer.Serialize(vo.Value);

            Assert.Equal(serializedVo1, serializedFloat1);
            Assert.Equal(serializedVo2, serializedFloat2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoJsonFloatVo.From(123);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":" + vo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonFloatVo.From(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterFloatVo.From(123);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":" + vo.Value + "}";

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

            var original = new EfCoreTestEntity { Id = EfCoreFloatVo.From(123) };
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

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbFloatVo.From(123) };
            using (var context = new DataConnection(
                SQLiteTools.GetDataProvider("SQLite.MS"),
                connection,
                disposeConnection: false))
            {
                context.CreateTable<LinqToDbTestEntity>();
                context.Insert(original);
            }
            using (var context = new DataConnection(
                SQLiteTools.GetDataProvider("SQLite.MS"),
                connection,
                disposeConnection: false))
            {
                var all = context.GetTable<LinqToDbTestEntity>().ToList();
                var retrieved = Assert.Single(all);
                Assert.Equal(original.Id, retrieved.Id);
            }
        }

        [Theory]
        [InlineData((float)123.45)]
        [InlineData("123.45")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonFloatVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonFloatVo>(id);
            Assert.Equal(NoJsonFloatVo.From(123.45f), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        public class TestDbContext : DbContext
        {
            public DbSet<EfCoreTestEntity> Entities { get; set; }

            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

             protected override void OnModelCreating(ModelBuilder modelBuilder)
             {
                 modelBuilder
                     .Entity<EfCoreTestEntity>(builder =>
                     {
                         builder
                             .Property(x => x.Id)
                             .HasConversion(new EfCoreFloatVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class EfCoreTestEntity
        {
            public EfCoreFloatVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Single)]
            [ValueConverter(ConverterType = typeof(LinqToDbFloatVo.LinqToDbValueConverter))]
            public LinqToDbFloatVo Id { get; set; }
        }
    }
}