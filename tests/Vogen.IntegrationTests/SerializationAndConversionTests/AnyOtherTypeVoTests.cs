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
using Vogen.IntegrationTests.SerializationAndConversionTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.IntegrationTests.SerializationAndConversionTests
{
    [ValueObject(underlyingType: typeof(Bar))]
    public partial struct AnotherFooVo { }

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
        public void CanSerializeToString_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonFooVo.From(_bar1);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            serializedFoo.Equals(serializedString).Should().BeTrue();
        }

        [Fact]
        public void CanDeserializeFromString_WithNewtonsoftJsonProvider()
        {
            var value = _bar1;
            var foo = NewtonsoftJsonFooVo.From(value);
            var serializedString = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFooVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromString_WithSystemTextJsonProvider()
        {
            var value = _bar1;
            var foo = SystemTextJsonFooVo.From(value);
            var serializedString = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo = SystemTextJsonSerializer.Deserialize<SystemTextJsonFooVo>(serializedString);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToString_WithBothJsonConverters()
        {
            var foo = BothJsonFooVo.From(_bar1);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedString1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedString2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedString1);
            Assert.Equal(serializedFoo2, serializedString2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonFooVo.From(_bar1);

            var serialized = SystemTextJsonSerializer.Serialize(foo);

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
            var foo = NoConverterFooVo.From(_bar1);

            var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
            var systemText = SystemTextJsonSerializer.Serialize(foo);

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

            var original = new TestEntity { Id = EfCoreFooVo.From(_bar1) };
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

            IEnumerable<DapperFooVo> results = await connection.QueryAsync<DapperFooVo>("SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(value, DapperFooVo.From(_bar1));
        }

        [Fact]
        public void TypeConverter_CanConvertToAndFrom()
        {
            var b = _bar1;
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonFooVo));
            
            object vo = converter.ConvertFrom(_bar1);
            //object id3 = converter.ConvertTo(id2, typeof(Bar));
            // object id = converter.ConvertFrom(value);
            //
            Assert.IsType<NoJsonFooVo>(vo);
            //
            Assert.Equal(NoJsonFooVo.From(_bar1), vo);
            //
            object reconverted = converter.ConvertTo(vo, typeof(Bar));
            Assert.IsType<Bar>(reconverted);
            Assert.Equal(((NoJsonFooVo)vo).Value, reconverted);
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
                             .HasConversion(new EfCoreFooVo.EfCoreValueConverter())
                             .ValueGeneratedNever();
                     })
                     // .Entity<Bar>(builder =>
                     // {
                     //     builder
                     //         .Property(x => x.Name)
                     //         .HasConversion<string>()
                     //         .ValueGeneratedNever();
                     //     builder
                     //         .Property(x => x.Age)
                     //         .HasConversion<int>()
                     //         .ValueGeneratedNever();
                     // })
                     //
                     ;
             }
        }

        public class TestEntity
        {
            public EfCoreFooVo Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonFooVo? Id { get; set; }
        }
    }
}