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
using BothJsonByteVo = Vogen.IntegrationTests.TestTypes.StructVos.BothJsonByteVo;
using ByteVo = Vogen.IntegrationTests.TestTypes.StructVos.ByteVo;
using DapperByteVo = Vogen.IntegrationTests.TestTypes.StructVos.DapperByteVo;
using EfCoreByteVo = Vogen.IntegrationTests.TestTypes.StructVos.EfCoreByteVo;
using NewtonsoftJsonByteVo = Vogen.IntegrationTests.TestTypes.StructVos.NewtonsoftJsonByteVo;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using NoConverterByteVo = Vogen.IntegrationTests.TestTypes.StructVos.NoConverterByteVo;
using NoJsonByteVo = Vogen.IntegrationTests.TestTypes.StructVos.NoJsonByteVo;
using SystemTextJsonByteVo = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonByteVo;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable ConvertToLocalFunction

namespace Vogen.IntegrationTests.SerializationAndConversionTests.StructVos
{
    [ValueObject(underlyingType: typeof(byte))]
    public partial struct AnotherByteVo { }

    public class ByteVoTests
    {
        [Fact]
        public void equality_between_same_value_objects()
        {
            ByteVo.From(18).Equals(ByteVo.From(18)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(18)).Should().BeTrue();

            (ByteVo.From(18) != ByteVo.From(19)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(19)).Should().BeFalse();

            ByteVo.From(18).Equals(ByteVo.From(18)).Should().BeTrue();
            (ByteVo.From(18) == ByteVo.From(18)).Should().BeTrue();

            var original = ByteVo.From(18);
            var other = ByteVo.From(18);

            ((original as IEquatable<ByteVo>).Equals(other)).Should().BeTrue();
            ((other as IEquatable<ByteVo>).Equals(original)).Should().BeTrue();
        }


        [Fact]
        public void equality_between_different_value_objects()
        {
            ByteVo.From(18).Equals(AnotherByteVo.From(18)).Should().BeFalse();
        }

        [Fact]
        public void CanSerializeToShort_WithNewtonsoftJsonProvider()
        {
            var vo = NewtonsoftJsonByteVo.From(123);

            string serializedVo = NewtonsoftJsonSerializer.SerializeObject(vo);
            string serializedShort = NewtonsoftJsonSerializer.SerializeObject(vo.Value);

            Assert.Equal(serializedVo, serializedShort);
        }

        [Fact]
        public void CanSerializeToShort_WithSystemTextJsonProvider()
        {
            var vo = SystemTextJsonByteVo.From(123);

            string serializedVo = SystemTextJsonSerializer.Serialize(vo);
            string serializedShort = SystemTextJsonSerializer.Serialize(vo.Value);

            serializedVo.Equals(serializedShort).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
        {
            byte value = 123;
            var vo = NewtonsoftJsonByteVo.From(value);
            var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedVo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonByteVo>(serializedShort);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanDeserializeFromShort_WithSystemTextJsonProvider()
        {
            byte value = 123;
            var vo = SystemTextJsonByteVo.From(value);
            var serializedShort = SystemTextJsonSerializer.Serialize(value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonByteVo>(serializedShort);

            Assert.Equal(vo, deserializedVo);
        }

        [Fact]
        public void CanSerializeToShort_WithBothJsonConverters()
        {
            var vo = BothJsonByteVo.From(123);

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
            var vo = NoJsonByteVo.From(123);

            var serialized = SystemTextJsonSerializer.Serialize(vo);

            var expected = "{\"Value\":" + vo.Value + "}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var vo = NoJsonByteVo.From(123);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(vo);

            var expected = $"\"{vo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var vo = NoConverterByteVo.From(123);

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

            var original = new TestEntity { Id = EfCoreByteVo.From(123) };
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

            IEnumerable<DapperByteVo> results = await connection.QueryAsync<DapperByteVo>("SELECT 123");

            var value = Assert.Single(results);
            Assert.Equal(DapperByteVo.From(123), value);
        }

        [Theory]
        [InlineData((byte) 123)]
        [InlineData("123")]
        public void TypeConverter_CanConvertToAndFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonByteVo));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonByteVo>(id);
            Assert.Equal(NoJsonByteVo.From(123), id);

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
                            .HasConversion(new EfCoreByteVo.EfCoreValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreByteVo Id { get; set; }
        }
    }
}