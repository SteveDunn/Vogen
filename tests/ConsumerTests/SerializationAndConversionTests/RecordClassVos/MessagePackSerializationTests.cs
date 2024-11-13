using MessagePack;
using MessagePack.Formatters;
using Vogen.IntegrationTests.TestTypes;
using Vogen.IntegrationTests.TestTypes.RecordClassVos;
using Bar = Vogen.IntegrationTests.TestTypes.Bar;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace ConsumerTests.SerializationAndConversionTests.RecordClassVos;

public class MessagePackSerializationTests
{
    public class FooFormatter : IMessagePackFormatter<Bar>
    {
        public Bar Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var age = reader.ReadInt32();
            var name = reader.ReadString();

            return new Bar
            {
                Age = age,
                Name = name!
            };
        }

        public void Serialize(ref MessagePackWriter writer, Bar value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Age);
            writer.Write(value.Name);
        }
    }

    private readonly MessagePackBoolVoMessagePackFormatter _boolVo = new MessagePackBoolVoMessagePackFormatter();
    private readonly MessagePackFloatVoMessagePackFormatter _floatVo = new MessagePackFloatVoMessagePackFormatter();
    private readonly MessagePackByteVoMessagePackFormatter _byteVo = new MessagePackByteVoMessagePackFormatter();
    private readonly MessagePackCharVoMessagePackFormatter _charVo = new MessagePackCharVoMessagePackFormatter();
    private readonly MessagePackDateOnlyVoMessagePackFormatter _dateOnlyVo = new MessagePackDateOnlyVoMessagePackFormatter();

    private readonly MessagePackDateTimeOffsetVoMessagePackFormatter _dateTimeOffsetVo =
        new MessagePackDateTimeOffsetVoMessagePackFormatter();

    private readonly MessagePackDateTimeVoMessagePackFormatter _dateTimeVo = new MessagePackDateTimeVoMessagePackFormatter();
    private readonly MessagePackDecimalVoMessagePackFormatter _decimalVo = new MessagePackDecimalVoMessagePackFormatter();
    private readonly MessagePackDoubleVoMessagePackFormatter _doubleVo = new MessagePackDoubleVoMessagePackFormatter();
    private readonly MessagePackFooVoMessagePackFormatter _fooVo = new MessagePackFooVoMessagePackFormatter();
    private readonly MessagePackGuidVoMessagePackFormatter _guidVo = new MessagePackGuidVoMessagePackFormatter();
    private readonly MessagePackIntVoMessagePackFormatter _intVo = new MessagePackIntVoMessagePackFormatter();
    private readonly MessagePackLongVoMessagePackFormatter _longVo = new MessagePackLongVoMessagePackFormatter();
    private readonly MessagePackShortVoMessagePackFormatter _shortVo = new MessagePackShortVoMessagePackFormatter();
    private readonly MessagePackStringVoMessagePackFormatter _stringVo = new MessagePackStringVoMessagePackFormatter();
    private readonly MessagePackTimeOnlyVoMessagePackFormatter _timeOnlyVo = new MessagePackTimeOnlyVoMessagePackFormatter();
    private readonly FooFormatter _fooFormatter = new FooFormatter();
    private readonly MessagePackSerializerOptions _options;

    public MessagePackSerializationTests()
    {
        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            [
                _fooFormatter,
                _boolVo,
                _floatVo,
                _byteVo,
                _charVo,
                _dateOnlyVo,
                _dateTimeOffsetVo,
                _dateTimeVo,
                _decimalVo,
                _doubleVo,
                _fooVo,
                _guidVo,
                _intVo,
                _longVo,
                _shortVo,
                _stringVo,
                _timeOnlyVo
            ],
            [MessagePack.Resolvers.StandardResolver.Instance]
        );

        _options = MessagePackSerializerOptions.Standard.WithResolver(customResolver);
    }

    [Fact]
    public void RoundTrip_Bool()
    {
        var vo = MessagePackBoolVo.From(true);

        var mp = MessagePackSerializer.Serialize(vo, _options);

        MessagePackBoolVo deserialised = MessagePackSerializer.Deserialize<MessagePackBoolVo>(mp, _options);

        vo.Value.Should().Be(deserialised.Value);
    }

    [Fact]
    public void RoundTrip_Byte()
    {
        byte value = 123;
        var vo = MessagePackByteVo.From(value);
        var mp = MessagePackSerializer.Serialize(value, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackByteVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Char()
    {
        char value = 'a';
        var vo = MessagePackCharVo.From(value);
        var mp = MessagePackSerializer.Serialize(value, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackCharVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip()
    {
        var value = "ABC";
        var vo = MessagePackStringVo.From(value);
        var mp = MessagePackSerializer.Serialize(value, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackStringVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTimeOffset()
    {
        var vo = MessagePackDateTimeOffsetVo.From(Primitives.DateTimeOffset1);
        var mp = MessagePackSerializer.Serialize(vo, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackDateTimeOffsetVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_DateTime()
    {
        var vo = MessagePackDateTimeVo.From(Primitives.DateTime1);
        var mp = MessagePackSerializer.Serialize(vo, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackDateTimeVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Decimal()
    {
        var vo = MessagePackDecimalVo.From(123.45m);

        var mp = MessagePackSerializer.Serialize(vo, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackDecimalVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Double()
    {
        var vo = MessagePackDoubleVo.From(123.45d);

        var mp = MessagePackSerializer.Serialize(vo, _options);

        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackDoubleVo>(mp, _options);

        Assert.Equal(vo, deserializedVo);
    }

    [Fact]
    public void RoundTrip_Float()
    {
        var vo = MessagePackFloatVo.From(123.45f);

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackFloatVo>(mp, _options);

        deserializedVo.Value.Should().Be(123.45f);
    }

    [Fact]
    public void RoundTrip_Guid()
    {
        var vo = MessagePackGuidVo.From(Primitives.Guid1);

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackGuidVo>(mp, _options);

        deserializedVo.Value.Should().Be(Primitives.Guid1);
    }

    [Fact]
    public void RoundTrip_Int()
    {
        var vo = MessagePackLongVo.From(123L);

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackIntVo>(mp, _options);

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_ShortMessagePackProvider()
    {
        var vo = MessagePackShortVo.From(123);

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackShortVo>(mp, _options);

        deserializedVo.Value.Should().Be(123);
    }

    [Fact]
    public void RoundTrip_String()
    {
        var vo = MessagePackStringVo.From("aaa");

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackStringVo>(mp, _options);

        deserializedVo.Value.Should().Be("aaa");
    }

    [Fact]
    public void RoundTrip_TimeOnly()
    {
        var vo = MessagePackTimeOnlyVo.From(Primitives.Time1);

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackTimeOnlyVo>(mp, _options);

        deserializedVo.Value.Should().Be(Primitives.Time1);
    }

    [Fact]
    public void RoundTrip_Foo()
    {
        var vo = MessagePackFooVo.From(new Bar(42, "Fred"));

        var mp = MessagePackSerializer.Serialize(vo, _options);
        var deserializedVo = MessagePackSerializer.Deserialize<MessagePackFooVo>(mp, _options);

        deserializedVo.Value.Age.Should().Be(42);
        deserializedVo.Value.Name.Should().Be("Fred");
    }
}