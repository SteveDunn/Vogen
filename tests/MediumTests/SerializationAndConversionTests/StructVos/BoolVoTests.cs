#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Vogen.IntegrationTests.TestTypes.StructVos;
// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable ConvertToLocalFunction

namespace Vogen.IntegrationTests.SerializationAndConversionTests.StructVos
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
            var vo = NewtonsoftJsonBoolVo.From(true);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            string serializedBool = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            Assert.Equal(serializedVo, serializedBool);
        }

        [Fact]
        public void CanSerializeToShort_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonBoolVo.From(true);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedShort = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedShort).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
        {
            bool value = true;
            var vo = NewtonsoftJsonBoolVo.From(value);
            var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonBoolVo>(serializedShort);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromShort_WithSystemTextJsonProvider()
        {
            bool value = true;
            var vo = SystemTextJsonBoolVo.From(value);
            var serializedShort = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonBoolVo>(serializedShort);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToShort_WithBothJsonConverters()
        {
            var vo = BothJsonBoolVo.From(true);

            var serializedVo1 = NewtonsoftJsonSerializer.SerializeObject(vo);
            var serializedShort1 = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            var serializedVo2 = SystemTextJsonSerializer.Serialize(vo);
            var serializedShort2 = SystemTextJsonSerializer.Serialize(vo.Value);

            Assert.Equal(serializedVo1, serializedShort1);
            Assert.Equal(serializedVo2, serializedShort2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoJsonBoolVo.From(true);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":true}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonBoolVo.From(true);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{(bool)vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterBoolVo.From(true);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

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

            var original = new EfCoreTestEntity { Id = EfCoreBoolVo.From(true) };
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

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { Id = LinqToDbBoolVo.From(true) };
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
                            .HasConversion(new EfCoreBoolVo.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class EfCoreTestEntity
        {
            public EfCoreBoolVo Id { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.Boolean)]
            [ValueConverter(ConverterType = typeof(LinqToDbBoolVo.LinqToDbValueConverter))]
            public LinqToDbBoolVo Id { get; set; }
        }
    }
}