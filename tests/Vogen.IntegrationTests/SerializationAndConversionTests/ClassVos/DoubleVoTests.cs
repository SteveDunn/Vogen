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
using Vogen.IntegrationTests.TestTypes.ClassVos;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos
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
            var vo = NewtonsoftJsonDoubleVo.From(123D);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            string serializedLong = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            Assert.Equal(serializedVo, serializedLong);
        }

        [Fact]
        public void CanSerializeToLong_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonDoubleVo.From(123D);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedLong = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedLong).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
        {
            var value = 123D;
            var vo = NewtonsoftJsonDoubleVo.From(value);
            var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDoubleVo>(serializedLong);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromLong_WithSystemTextJsonProvider()
        {
            var value = 123D;
            var vo = SystemTextJsonDoubleVo.From(value);
            var serializedLong = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDoubleVo>(serializedLong);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToLong_WithBothJsonConverters()
        {
            var vo = BothJsonDoubleVo.From(123D);

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
            var vo = NoJsonDoubleVo.From(123D);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":" + vo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonDoubleVo.From(123D);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterDoubleVo.From(123D);

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

            var original = new EfCoreTestEntity { Id = EfCoreDoubleVo.From(123D) };
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

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbDoubleVo.From(123) };
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
        
        [Fact]
        public void RoundTrip_WithNsj()
        {
            var vo = NewtonsoftJsonDoubleVo.From(123.45);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDoubleVo>(serializedVo)!;

            deserializedVo.Value.Should().Be(123.45);
        }

        [Fact]
        public void RoundTrip_WithStj()
        {
            var vo = SystemTextJsonDoubleVo.From(123.45);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDoubleVo>(serializedVo)!;

            deserializedVo.Value.Should().Be(123.45);
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
                             .HasConversion(new EfCoreDoubleVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class EfCoreTestEntity
        {
            public EfCoreDoubleVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Double)]
            [ValueConverter(ConverterType = typeof(LinqToDbDoubleVo.LinqToDbValueConverter))]
            public LinqToDbDoubleVo Id { get; set; }
        }
    }
}