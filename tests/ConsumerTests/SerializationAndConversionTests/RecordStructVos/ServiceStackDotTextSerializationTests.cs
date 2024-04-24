using ServiceStack.Text;
using Vogen.IntegrationTests.TestTypes;
using Vogen.IntegrationTests.TestTypes.RecordStructVos;
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable RedundantSuppressNullableWarningExpression

namespace ConsumerTests.SerializationAndConversionTests.RecordStructVos;

public class ServiceStackDotTextSerializationTests
{
    [Fact]
    public void RoundTrip_Bool_WithSsdtProvider()
    {
        var vo = SsdtBoolVo.From(true);

        string json = JsonSerializer.SerializeToString(vo);

        SsdtBoolVo deserialised = JsonSerializer.DeserializeFromString<SsdtBoolVo>(json);

        vo.Value.Should().Be(deserialised.Value);
    }

    [Fact]
    public void RoundTrip_Byte_WithSsdtProvider()
    {
        byte value = 123;
        var vo = SsdtByteVo.From(value);
        var json = JsonSerializer.SerializeToString(value);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtByteVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Char_WithSsdtProvider()
    {
        char value = 'a';
        var vo = SsdtCharVo.From(value);
        var json = JsonSerializer.SerializeToString(value);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtCharVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_WithSsdtProvider()
    {
        var value = "ABC";
        var vo = SsdtStringVo.From(value);
        var json = JsonSerializer.SerializeToString(value);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtStringVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTimeOffset_WithSsdtProvider()
    {
        var vo = SsdtDateTimeOffsetVo.From(Primitives.DateTimeOffset1);
        var json = JsonSerializer.SerializeToString(vo);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtDateTimeOffsetVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTime_WithSsdtProvider()
    {
        var vo = SsdtDateTimeVo.From(Primitives.DateTime1);
        var json = JsonSerializer.SerializeToString(vo);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtDateTimeVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Decimal_WithSsdtProvider()
    {
        var vo = SsdtDecimalVo.From(123.45m);

        var json = JsonSerializer.SerializeToString(vo);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtDecimalVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Double_WithSsdtProvider()
    {
        var vo = SsdtDoubleVo.From(123.45d);

        var json = JsonSerializer.SerializeToString(vo);

        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtDoubleVo>(json);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Float_WithSsdtProvider()
    {
        var vo = SsdtFloatVo.From(123.45f);

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtFloatVo>(serializedVo)!;

        deserializedVo.Value.Should().Be(123.45f);
    }

    [Fact]
    public void RoundTrip_Guid_WithSsdtProvider()
    {
        var vo = SsdtGuidVo.From(Primitives.Guid1);

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtGuidVo>(serializedVo)!;

        deserializedVo.Value.Should().Be(Primitives.Guid1);
    }

    [Fact]
    public void RoundTrip_Int_WithSsdtProvider()
    {
        var vo = SsdtLongVo.From(123L);

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtIntVo>(serializedVo)!;

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_ShortSsdtProvider()
    {
        var vo = SsdtShortVo.From(123);

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtShortVo>(serializedVo)!;

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_String_WithSsdtProvider()
    {
        var vo = SsdtStringVo.From("aaa");

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtStringVo>(serializedVo)!;

        deserializedVo.Value.Should().Be("aaa");
    }

    [Fact]
    public void RoundTrip_TimeOnly_WithSsdtProvider()
    {
        var vo = SsdtTimeOnlyVo.From(Primitives.Time1);

        string serializedVo = JsonSerializer.SerializeToString(vo);
        var deserializedVo = JsonSerializer.DeserializeFromString<SsdtTimeOnlyVo>(serializedVo)!;

        deserializedVo.Value.Should().Be(Primitives.Time1);
    }
}