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
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Vogen.IntegrationTests.TestTypes.StructVos;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.StructVos
{
    [ValueObject(underlyingType: typeof(decimal))]
    public partial struct AnotherDecimalVo { }

    public class DecimalVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            DecimalVo.From(18).Equals(DecimalVo.From(18)).Should().BeTrue();
            (DecimalVo.From(18) == DecimalVo.From(18)).Should().BeTrue();

            (DecimalVo.From(18) != DecimalVo.From(19)).Should().BeTrue();
            (DecimalVo.From(18) == DecimalVo.From(19)).Should().BeFalse();

            DecimalVo.From(18).Equals(DecimalVo.From(18)).Should().BeTrue();
            (DecimalVo.From(18) == DecimalVo.From(18)).Should().BeTrue();

            var original = DecimalVo.From(18);
            var other = DecimalVo.From(18);

            ((original as IEquatable<DecimalVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<DecimalVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            DecimalVo.From(18).Equals(AnotherDecimalVo.From(18)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToLong_WithNewtonsoftJsonProvider()
        {
            var vo = NewtonsoftJsonDecimalVo.From(123M);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            string serializedLong = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            Assert.Equal(serializedVo, serializedLong);
        }

        [Fact]
        public void CanSerializeToLong_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonDecimalVo.From(123M);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedLong = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedLong).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
        {
            var value = 123M;
            var vo = NewtonsoftJsonDecimalVo.From(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDecimalVo>(serializedLong);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithSystemTextJsonProvider()
        {
            var value = 123M;
            var vo = SystemTextJsonDecimalVo.From(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDecimalVo>(serializedLong);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToLong_WithBothJsonConverters()
        {
            var vo = BothJsonDecimalVo.From(123M);

            var serializedVo1 = NewtonsoftJsonSerializer.SerializeObject(vo);
            var serializedLong1 = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            var serializedVo2 = SystemTextJsonSerializer.Serialize(vo);
            var serializedLong2 = SystemTextJsonSerializer.Serialize(vo.Value);

            Assert.Equal(serializedVo1, serializedLong1);
            Assert.Equal(serializedVo2, serializedLong2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoJsonDecimalVo.From(123M);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":" + vo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonDecimalVo.From(123M);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterDecimalVo.From(123M);

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

            var original = new EfCoreTestEntity { Id = EfCoreDecimalVo.From(123M) };
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

            IEnumerable<DapperDecimalVo> results = await connection.QueryAsync<DapperDecimalVo>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(DapperDecimalVo.From(123M), value);
        }

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbDecimalVo.From(123) };
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

        [Fact]
        public void TypeConverter_CanConvertToAndFromDecimal()
        {
            decimal value = 123m;
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDecimalVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDecimalVo>(id);
            Assert.Equal(NoJsonDecimalVo.From(123M), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Theory]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDecimalVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDecimalVo>(id);
            Assert.Equal(NoJsonDecimalVo.From(123M), id);

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
                             .HasConversion(new EfCoreDecimalVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class EfCoreTestEntity
        {
            public EfCoreDecimalVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Decimal)]
            [ValueConverter(ConverterType = typeof(LinqToDbDecimalVo.LinqToDbValueConverter))]
            public LinqToDbDecimalVo Id { get; set; }
        }
    }
}