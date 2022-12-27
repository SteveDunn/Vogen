#if NET6_0_OR_GREATER

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
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Vogen.IntegrationTests.TestTypes.ClassVos;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;
// ReSharper disable EqualExpressionComparison
// ReSharper disable RedundantCast
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable SuspiciousTypeConversion.Global

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos
{
    [ValueObject(underlyingType: typeof(DateOnly))]
    public readonly partial struct AnotherDateOnlyVo { }

    public class DateOnlyVoTests
    {
        private static readonly DateOnly _date1 = new DateOnly(1970, 6, 10);
        private static readonly DateOnly _date2 = new DateOnly(2022, 12, 27);
        
        [Fact]
        public void equality_between_same_value_objects()
        {
            DateOnlyVo.From(_date1).Equals(DateOnlyVo.From(_date1)).Should().BeTrue();
            (DateOnlyVo.From(_date1) == DateOnlyVo.From(_date1)).Should().BeTrue();

            (DateOnlyVo.From(_date1) != DateOnlyVo.From(_date2)).Should().BeTrue();
            (DateOnlyVo.From(_date1) == DateOnlyVo.From(_date2)).Should().BeFalse();

            DateOnlyVo.From(_date1).Equals(DateOnlyVo.From(_date1)).Should().BeTrue();
            (DateOnlyVo.From(_date1) == DateOnlyVo.From(_date1)).Should().BeTrue();

            var original = DateOnlyVo.From(_date1);
            var other = DateOnlyVo.From(_date1);

            ((original as IEquatable<DateOnlyVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<DateOnlyVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            DateOnlyVo.From(_date1).Equals(AnotherDateOnlyVo.From(_date1)).Should().BeFalse();
        }

#if NET7_0_OR_GREATER
        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var g1 = NewtonsoftJsonDateOnlyVo.From(_date1);

            string serialized = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serialized, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonDateOnlyVo.From(_date1);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedString = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _date1;
            var vo = NewtonsoftJsonDateOnlyVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDateOnlyVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }
        
        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _date1;
            var vo = SystemTextJsonDateOnlyVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDateOnlyVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var vo = BothJsonDateOnlyVo.From(_date1);

            var serializedVo1 = NewtonsoftJsonSerializer.SerializeObject(vo);
            var serializedString1 = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            var serializedVo2 = SystemTextJsonSerializer.Serialize(vo);
            var serializedString2 = SystemTextJsonSerializer.Serialize(vo.Value);

            Assert.Equal(serializedVo1, serializedString1);
            Assert.Equal(serializedVo2, serializedString2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoJsonDateOnlyVo.From(_date1);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + _date1.ToString("O") + "\"}";

            serialized.Should().Be(expected);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonDateOnlyVo.From(_date1);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{_date1:o}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterDateOnlyVo.From(_date1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + _date1.ToString("O") + "\"}";

            newtonsoft.Should().Be(expected);
            systemText.Should().Be(expected);
        }
#endif
        
        [Fact]
        public void WhenEfCoreValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new EfCoreTestEntity { Id = EfCoreDateOnlyVo.From(_date1) };
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

            IEnumerable<DapperDateOnlyVo> results = await connection.QueryAsync<DapperDateOnlyVo>("SELECT '2022-01-15'");

            DapperDateOnlyVo actual = Assert.Single(results);

            var expected = DapperDateOnlyVo.From(new DateOnly(2022,01,15));
            actual.Should().Be(expected);
        }

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbDateOnlyVo.From(_date1) };
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
        [InlineData("2022-01-15")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonDateOnlyVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonDateOnlyVo>(id);
            Assert.Equal(NoJsonDateOnlyVo.From(DateOnly.ParseExact(value, "O", CultureInfo.InvariantCulture)), id);

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
                             .HasConversion(new EfCoreDateOnlyVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class EfCoreTestEntity
        {
            public EfCoreDateOnlyVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Date)]
            [ValueConverter(ConverterType = typeof(LinqToDbDateOnlyVo.LinqToDbValueConverter))]
            public LinqToDbDateOnlyVo Id { get; set; }
        }
    }
}

#endif