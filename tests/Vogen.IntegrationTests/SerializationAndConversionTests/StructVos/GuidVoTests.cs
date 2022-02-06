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
using BothJsonGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.BothJsonGuidVo;
using DapperGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.DapperGuidVo;
using EfCoreGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.EfCoreGuidVo;
using GuidVo = Vogen.IntegrationTests.TestTypes.StructVos.GuidVo;
using NewtonsoftJsonGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.NewtonsoftJsonGuidVo;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using NoConverterGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.NoConverterGuidVo;
using NoJsonGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.NoJsonGuidVo;
using SystemTextJsonGuidVo = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonGuidVo;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.StructVos
{
    [ValueObject(underlyingType: typeof(Guid))]
    public partial struct AnotherGuidVo { }

    public class GuidVoTests
    {
        public static readonly Guid _guid1 = Guid.NewGuid();
        public static readonly Guid _guid2 = Guid.NewGuid();
        [Fact]
        public void equality_between_same_value_objects()
        {
            GuidVo.From(_guid1).Equals(GuidVo.From(_guid1)).Should().BeTrue();
            (GuidVo.From(_guid1) == GuidVo.From(_guid1)).Should().BeTrue();

            (GuidVo.From(_guid1) != GuidVo.From(_guid2)).Should().BeTrue();
            (GuidVo.From(_guid1) == GuidVo.From(_guid2)).Should().BeFalse();

            GuidVo.From(_guid1).Equals(GuidVo.From(_guid1)).Should().BeTrue();
            (GuidVo.From(_guid1) == GuidVo.From(_guid1)).Should().BeTrue();

            var original = GuidVo.From(_guid1);
            var other = GuidVo.From(_guid1);

            ((original as IEquatable<GuidVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<GuidVo>).Equals(original)).Should().BeTrue();
        }

        [Fact]
        public void equality_between_different_value_objects()
        {
            GuidVo.From(_guid1).Equals(AnotherGuidVo.From(_guid1)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToString_WithNewtonsoftJsonProvider()
        {
            var g1 = NewtonsoftJsonGuidVo.From(_guid1);

            string serializedGuid = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            Assert.Equal(serializedGuid, serializedString);
        }

        [Fact]
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonGuidVo.From(_guid1);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedString = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _guid1;
            var vo = NewtonsoftJsonGuidVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonGuidVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _guid1;
            var vo = SystemTextJsonGuidVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonGuidVo>(serializedString);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var vo = BothJsonGuidVo.From(_guid1);

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
            var vo = NoJsonGuidVo.From(_guid1);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + vo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonGuidVo.From(_guid1);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterGuidVo.From(_guid1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(vo);
            var systemText = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":\"" + vo.Value + "\"}";

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

            var original = new TestEntity { Id = EfCoreGuidVo.From(_guid1) };
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

            IEnumerable<DapperGuidVo> results = await connection.QueryAsync<DapperGuidVo>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(value, DapperGuidVo.From(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonGuidVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonGuidVo>(id);
            Assert.Equal(NoJsonGuidVo.From(Guid.Parse(value)), id);

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
                             .HasConversion(new EfCoreGuidVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     });
             }
        }

        public class TestEntity
        {
            public EfCoreGuidVo Id { get; set; }
        }
    }
}