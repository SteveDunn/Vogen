using FluentAssertions.Extensions;
using Vogen.IntegrationTests.TestTypes;
using Vogen.IntegrationTests.TestTypes.RecordStructVos;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace ConsumerTests.SerializationAndConversionTests.RecordStructVos;

public class BsonSerializationTests
{
    [Fact]
    public void RoundTrip_Bool()
    {
        var vo = BsonBoolVo.From(true);

        string json = BsonSerializerButUsesJson.Serialize(vo);

        BsonBoolVo deserialised = BsonSerializerButUsesJson.Deserialize<BsonBoolVo>(json);

        vo.Value.Should().Be(deserialised.Value);
    }

    [Fact]
    public void RoundTrip_Byte()
    {
        byte value = 123;
        var vo = BsonByteVo.From(value);
        var json = BsonSerializerButUsesJson.Serialize(value);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonByteVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Char()
    {
        char value = 'a';
        var vo = BsonCharVo.From(value);
        var json = BsonSerializerButUsesJson.Serialize(value);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonCharVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip()
    {
        var value = "ABC";
        var vo = BsonStringVo.From(value);
        var json = BsonSerializerButUsesJson.Serialize(value);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonStringVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTimeOffset()
    {
        var vo = BsonDateTimeOffsetVo.From(Primitives.DateTimeOffset1);
        var json = BsonSerializerButUsesJson.Serialize(vo);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonDateTimeOffsetVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTime()
    {
        var vo = BsonDateTimeVo.From(Primitives.DateTime1);
        var json = BsonSerializerButUsesJson.Serialize(vo);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonDateTimeVo>(json);
        
        // micro and non-second not populated in the Mongo C# driver
        vo.Value.Should().BeCloseTo(deserializedVo.Value, 1.Milliseconds());
    }

    [Fact]
    public void RoundTrip_Decimal()
    {
        var vo = BsonDecimalVo.From(123.45m);

        var json = BsonSerializerButUsesJson.Serialize(vo);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonDecimalVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Double()
    {
        var vo = BsonDoubleVo.From(123.45d);

        var json = BsonSerializerButUsesJson.Serialize(vo);

        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonDoubleVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Float()
    {
        var vo = BsonFloatVo.From(123.45f);

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonFloatVo>(serializedVo);

        deserializedVo.Value.Should().Be(123.45f);
    }

    [Fact]
    public void RoundTrip_Guid()
    {
        var vo = BsonGuidVo.From(Primitives.Guid1);

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonGuidVo>(serializedVo);

        deserializedVo.Value.Should().Be(Primitives.Guid1);
    }

    [Fact]
    public void RoundTrip_Int()
    {
        var vo = BsonLongVo.From(123L);

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonIntVo>(serializedVo);

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_ShortBsonProvider()
    {
        var vo = BsonShortVo.From(123);

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonShortVo>(serializedVo);

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_String()
    {
        var vo = BsonStringVo.From("aaa");

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonStringVo>(serializedVo);

        deserializedVo.Value.Should().Be("aaa");
    }

    [SkippableFact]
    public void RoundTrip_TimeOnly()
    {
        Skip.If(true, "Unsupported in the C# driver - https://jira.mongodb.org/browse/CSHARP-3717");
        var vo = BsonTimeOnlyVo.From(Primitives.Time1);

        string serializedVo = BsonSerializerButUsesJson.Serialize(vo);
        var deserializedVo = BsonSerializerButUsesJson.Deserialize<BsonTimeOnlyVo>(serializedVo);

        deserializedVo.Value.Should().Be(Primitives.Time1);
    }
}