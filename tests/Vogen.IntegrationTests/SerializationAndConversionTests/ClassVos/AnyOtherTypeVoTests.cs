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
using Vogen.IntegrationTests.TestTypes.ClassVos;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.ClassVos
{
    [ValueObject(underlyingType: typeof(Bar))]
    public partial class AnotherFooVo { }

    public class AnyOtherTypeVoTests
    {
        public static readonly Bar _bar1 = new Bar(42, "Fred");
        public static readonly Bar _wilma = new Bar(52, "Wilma");
        [Fact]
        public void equality_between_same_value_objects()
        {
            FooVo.From(_bar1).Equals(FooVo.From(_bar1)).Should().BeTrue();
            (FooVo.From(_bar1) == FooVo.From(_bar1)).Should().BeTrue();

            (FooVo.From(_bar1) != FooVo.From(_wilma)).Should().BeTrue();
            (FooVo.From(_bar1) == FooVo.From(_wilma)).Should().BeFalse();

            FooVo.From(_bar1).Equals(FooVo.From(_bar1)).Should().BeTrue();
            (FooVo.From(_bar1) == FooVo.From(_bar1)).Should().BeTrue();

            var original = FooVo.From(_bar1);
            var other = FooVo.From(_bar1);

            ((original as IEquatable<FooVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<FooVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            FooVo.From(_bar1).Equals(AnotherFooVo.From(_bar1)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            NewtonsoftJsonFooVo g1 = NewtonsoftJsonFooVo.From(_bar1);

            string serializedGuid = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serializedGuid, serializedString);
        }

        [Fact]
        public void CanSerializeClassToString_WithNewtonsoftJsonProvider()
        {
            NewtonsoftJsonFooVoClass g1 = NewtonsoftJsonFooVoClass.From(_bar1);

            string serializedGuid = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serializedGuid, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonFooVo.From(_bar1);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedString = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _bar1;
            var vo = NewtonsoftJsonFooVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFooVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromStringClass_WithNewtonsoftJsonProvider()
        {
            var value = _bar1;
            var vo = NewtonsoftJsonFooVoClass.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFooVoClass>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _bar1;
            var vo = SystemTextJsonFooVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonFooVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var vo = BothJsonFooVo.From(_bar1);

            var serializedVo1 = NewtonsoftJsonSerializer.SerializeObject(vo);
            var serializedString1 = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            var serializedVo2 = SystemTextJsonSerializer.Serialize(vo);
            var serializedString2 = SystemTextJsonSerializer.Serialize(vo.Value);

            Assert.Equal(serializedVo1, serializedString1);
            Assert.Equal(serializedVo2, serializedString2);
        }

        [Fact]
        public void CanSerializeToStringClass_WithBothJsonConverters()
        {
            var vo = BothJsonFooVoClass.From(_bar1);

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
            var vo = NoJsonFooVo.From(_bar1);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":{\"Age\":42,\"Name\":\"Fred\"}}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverterClass_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoJsonFooVoClass.From(_bar1);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":{\"Age\":42,\"Name\":\"Fred\"}}";

            Assert.Equal(expected, serialized);
        }

        /// <summary>
        /// There is no way for newtonsoft, via a type converter, to convert
        /// the underlying non-native type to json.
        /// </summary>
        [Fact]
        public void WithTypeConverterButNoJsonConverters_NewtonsoftSerializesWithValueProperty()
        {
            NoJsonFooVo foo = NoJsonFooVo.From(_bar1);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = "{\"Value\":{\"Age\":42,\"Name\":\"Fred\"}}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WithTypeConverterButNoJsonConverters_SystemTextJsonSerializesWithValueProperty()
        {
            var vo = NoConverterFooVo.From(_bar1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":{\"Age\":42,\"Name\":\"Fred\"}}";

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

            var original = new EfCoreTestEntity { FooField = EfCoreFooVo.From(_bar1) };
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
                Assert.Equal(original.FooField, retrieved.FooField);
            }
        }

        [Fact]
        public async Task WhenDapperValueConverterUsesValueConverter()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            IEnumerable<DapperFooVo> results = await connection.QueryAsync<DapperFooVo>("SELECT '{\"Age\":42,\"Name\":\"Fred\"}'");

            var value = Assert.Single(results);
            Assert.Equal(value, DapperFooVo.From(_bar1));
        }

        [Fact]
        public void WhenLinqToDbValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var original = new LinqToDbTestEntity { FooField = LinqToDbFooVo.From(_bar1) };
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
                Assert.Equal(original.FooField, retrieved.FooField);
            }
        }

        [Fact]
        public void TypeConverter_CanConvertToAndFrom()
        {
            var b = _bar1;
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonFooVo));

            object vo = converter.ConvertFrom(_bar1);

            Assert.IsType<NoJsonFooVo>(vo);

            Assert.Equal(NoJsonFooVo.From(_bar1), vo);

            object reconverted = converter.ConvertTo(vo, typeof(Bar));
            Assert.IsType<Bar>(reconverted);
            Assert.Equal(((NoJsonFooVo) vo).Value, reconverted);
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
                            .Property(x => x.FooField)
                            .HasConversion(new EfCoreFooVo.EfCoreValueConverter());
                    });
            }
        }

        public class EfCoreTestEntity
        {
            public int Id { get; set; }

            public EfCoreFooVo FooField { get; set; }
        }

        public class LinqToDbTestEntity
        {
            [Column(DataType = DataType.VarChar)]
            [ValueConverter(ConverterType = typeof(LinqToDbFooVo.LinqToDbValueConverter))]
            public LinqToDbFooVo FooField { get; set; }
        }
    }
}