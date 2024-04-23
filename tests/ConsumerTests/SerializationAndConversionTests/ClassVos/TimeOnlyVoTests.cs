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
using ServiceStack.Text;

// ReSharper disable EqualExpressionComparison
// ReSharper disable RedundantCast
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable SuspiciousTypeConversion.Global

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos
{
    [ValueObject(underlyingType: typeof(TimeOnly))]
    public readonly partial struct AnotherTimeOnlyVo { }

    public class TimeOnlyVoTests
    {
        private static readonly TimeOnly _time1 = new TimeOnly(13, 12, 59, 123);
        private static readonly TimeOnly _time2 = new TimeOnly(1, 59, 58, 123);
        
        [Fact]
        public void equality_between_same_value_objects()
        {
            TimeOnlyVo.From(_time1).Equals(TimeOnlyVo.From(_time1)).Should().BeTrue();
            (TimeOnlyVo.From(_time1) == TimeOnlyVo.From(_time1)).Should().BeTrue();

            (TimeOnlyVo.From(_time1) != TimeOnlyVo.From(_time2)).Should().BeTrue();
            (TimeOnlyVo.From(_time1) == TimeOnlyVo.From(_time2)).Should().BeFalse();

            TimeOnlyVo.From(_time1).Equals(TimeOnlyVo.From(_time1)).Should().BeTrue();
            (TimeOnlyVo.From(_time1) == TimeOnlyVo.From(_time1)).Should().BeTrue();

            var original = TimeOnlyVo.From(_time1);
            var other = TimeOnlyVo.From(_time1);

            ((original as IEquatable<TimeOnlyVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<TimeOnlyVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            TimeOnlyVo.From(_time1).Equals(AnotherTimeOnlyVo.From(_time1)).Should().BeFalse();
        }

#if NET7_0_OR_GREATER
        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var g1 = NewtonsoftJsonTimeOnlyVo.From(_time1);

            string serialized = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serialized, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonTimeOnlyVo.From(_time1);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedString = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedString).Should().BeTrue();
        }
        
        [Fact]
        public void RoundTrip_WithSsdtj()
        {
            var vo = SsdtTimeOnlyVo.From(_time1);
            
            global::ServiceStack.Text.JsConfig<SsdtTimeOnlyVo>.DeSerializeFn = v => SsdtTimeOnlyVo.Parse(v, CultureInfo.InvariantCulture);
            global::ServiceStack.Text.JsConfig<SsdtTimeOnlyVo>.SerializeFn = v => v.Value.ToString("o", CultureInfo.InvariantCulture);

            string serializedVo = JsonSerializer.SerializeToString(vo);
            var deserializedVo = JsonSerializer.DeserializeFromString<SsdtTimeOnlyVo>(serializedVo)!;

            deserializedVo.Value.Should().Be(_time1);
        }
        

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _time1;
            var vo = NewtonsoftJsonTimeOnlyVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonTimeOnlyVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }
        
        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _time1;
            var vo = SystemTextJsonTimeOnlyVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonTimeOnlyVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var vo = BothJsonTimeOnlyVo.From(_time1);

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
            var vo = NoJsonTimeOnlyVo.From(_time1);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + _time1.ToString("o") + "\"}";

            serialized.Should().Be(expected);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonTimeOnlyVo.From(_time1);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{_time1:o}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterTimeOnlyVo.From(_time1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + _time1.ToString("o") + "\"}";

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

            var original = new EfCoreTestEntity { Id = EfCoreTimeOnlyVo.From(_time1) };
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

            IEnumerable<DapperTimeOnlyVo> results = await connection.QueryAsync<DapperTimeOnlyVo>("SELECT '13:12:11.999'");

            DapperTimeOnlyVo actual = Assert.Single(results);

            var expected = DapperTimeOnlyVo.From(new TimeOnly(13,12,11, 999));
            actual.Should().Be(expected);
        }

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbTimeOnlyVo.From(_time1) };
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
        [InlineData("13:12:11.123")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonTimeOnlyVo));
            var voAsObject = converter.ConvertFrom(value);
            
            Assert.IsType<NoJsonTimeOnlyVo>(voAsObject);

            NoJsonTimeOnlyVo expected = NoJsonTimeOnlyVo.From(TimeOnly.Parse(value, CultureInfo.InvariantCulture));
            
            Assert.Equal(expected, voAsObject);

            var voBackAsString = converter.ConvertTo(voAsObject, value.GetType());
            
            Assert.Equal(TimeOnly.Parse(value, CultureInfo.InvariantCulture).ToString("o"), voBackAsString);
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
                             .HasConversion(new EfCoreTimeOnlyVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class EfCoreTestEntity
        {
            public EfCoreTimeOnlyVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Time)]
            [ValueConverter(ConverterType = typeof(LinqToDbTimeOnlyVo.LinqToDbValueConverter))]
            public LinqToDbTimeOnlyVo Id { get; set; }
        }
    }
}

#endif