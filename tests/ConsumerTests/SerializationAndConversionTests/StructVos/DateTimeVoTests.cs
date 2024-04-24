#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Vogen.IntegrationTests.TestTypes.StructVos;
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

namespace Vogen.IntegrationTests.SerializationAndConversionTests.StructVos;

[ValueObject(underlyingType: typeof(DateTime))]
public readonly partial struct AnotherDateTimeVo { }

public class DateTimeVoTests
{
    private static readonly DateTime _date1 = new DateTime(1970, 6, 10, 14, 01, 02, DateTimeKind.Utc) + TimeSpan.FromTicks(12345678);
    private static readonly DateTime _date2 = DateTime.Now.AddMinutes(42.69);
        
    [Fact]
    public void equality_between_same_value_objects()
    {
        DateTimeVo.From(_date1).Equals(DateTimeVo.From(_date1)).Should().BeTrue();
        (DateTimeVo.From(_date1) == DateTimeVo.From(_date1)).Should().BeTrue();

        (DateTimeVo.From(_date1) != DateTimeVo.From(_date2)).Should().BeTrue();
        (DateTimeVo.From(_date1) == DateTimeVo.From(_date2)).Should().BeFalse();

        DateTimeVo.From(_date1).Equals(DateTimeVo.From(_date1)).Should().BeTrue();
        (DateTimeVo.From(_date1) == DateTimeVo.From(_date1)).Should().BeTrue();

        var original = DateTimeVo.From(_date1);
        var other = DateTimeVo.From(_date1);

        ((original as IEquatable<DateTimeVo>).Equals(other)).Should().BeTrue();
        ((other as IEquatable<DateTimeVo>).Equals(original)).Should().BeTrue();
    }

    [Fact]
    public void equality_between_different_value_objects()
    {
        DateTimeVo.From(_date1).Equals(AnotherDateTimeVo.From(_date1)).Should().BeFalse();
    }

    [Fact]
    public void CanSerializeToString_WithNewtonsoftJsonProvider()
    {
        var g1 = NewtonsoftJsonDateTimeVo.From(_date1);

        string serialized = NewtonsoftJsonSerializer.SerializeObject(g1);
        string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

        Assert.Equal(serialized, serializedString);
    }

    [Fact]
    public void CanSerializeToString_WithSystemTextJsonProvider()
    {
        var vo = SystemTextJsonDateTimeVo.From(_date1);

        string serializedVo = SystemTextJsonSerializer.Serialize(vo);
        string serializedString = SystemTextJsonSerializer.Serialize(vo.Value);

        serializedVo.Equals(serializedString).Should().BeTrue();
    }

    [Fact]
    public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
    {
        var value = _date1;
        var vo = NewtonsoftJsonDateTimeVo.From(value);
        var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDateTimeVo>(serializedString);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void CanDeserializeFromString_WithSystemTextJsonProvider()
    {
        var value = _date1;
        var vo = SystemTextJsonDateTimeVo.From(value);
        var serializedString = SystemTextJsonSerializer.Serialize(value);

        var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDateTimeVo>(serializedString);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void CanSerializeToString_WithBothJsonConverters()
    {
        var vo = BothJsonDateTimeVo.From(_date1);

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
        var vo = NoJsonDateTimeVo.From(_date1);

        var serialized = SystemTextJsonSerializer.Serialize(vo);

        var expected = "{\"Value\":\"" + _date1.ToString("O") + "\"}";

        serialized.Should().Be(expected);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var vo = NoJsonDateTimeVo.From(_date1);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

        var expected = $"\"{_date1:o}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var vo = NoConverterDateTimeVo.From(_date1);

        var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
        var systemText = SystemTextJsonSerializer.Serialize(vo);

        var expected = "{\"Value\":\"" + _date1.ToString("O") + "\"}";

        newtonsoft.Should().Be(expected);
        systemText.Should().Be(expected);
    }

    [Fact]
    public void WhenEfCoreValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new EfCoreTestEntity { Id = EfCoreDateTimeVo.From(_date1) };
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

        IEnumerable<DapperDateTimeVo> results =
            await connection.QueryAsync<DapperDateTimeVo>("SELECT '2022-01-15 19:08:49.5413764'");

        DapperDateTimeVo actual = Assert.Single(results);

        var expected =
            DapperDateTimeVo.From(new DateTime(2022, 01, 15, 19, 08, 49, DateTimeKind.Utc).AddTicks(5413764));
        actual.Should().Be(expected);
    }

    [Fact]
    public void WhenLinqToDbValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var original = new LinqToDbTestEntity { Id = LinqToDbDateTimeVo.From(_date1) };
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
    [InlineData("2022-01-15T19:08:49.5413764+00:00")]
    public void TypeConverter_CanConvertToAndFrom(string inputText)
    {
        var sut = TypeDescriptor.GetConverter(typeof(NoJsonDateTimeVo));
        
        var vo = sut.ConvertFrom(inputText);
        Assert.IsType<NoJsonDateTimeVo>(vo);

        var inputAsDateTime = DateTime.ParseExact(inputText, "O", CultureInfo.InvariantCulture);
        Assert.Equal(NoJsonDateTimeVo.From(inputAsDateTime), vo);

        // local time string
        string reconverted = (string)sut.ConvertTo(vo, typeof(string))!;
        DateTime actual = DateTime.ParseExact(reconverted, "O", CultureInfo.InvariantCulture); 
        
        Assert.Equal(actual, inputAsDateTime);
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
                        .HasConversion(new EfCoreDateTimeVo.EfCoreValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class EfCoreTestEntity
    {
        public EfCoreDateTimeVo Id { get; set; }
    }

    public class LinqToDbTestEntity
    {
        [Column(DataType = DataType.DateTime)]
        [ValueConverter(ConverterType = typeof(LinqToDbDateTimeVo.LinqToDbValueConverter))]
        public LinqToDbDateTimeVo Id { get; set; }
    }
}