#nullable disable
using ConsumerTests;

// ReSharper disable FunctionComplexityOverflow

namespace MediumTests.SerializationAndConversionTests;


public partial class ComplexSerializationTests
{
    [Fact]
    public void Bson_CanSerializeAndDeserialize()
    {
        var complex = new ComplexBson();


        string serialized = BsonSerializerButUsesJson.Serialize(complex);
        ComplexBson deserialized = BsonSerializerButUsesJson.Deserialize<ComplexBson>(serialized);

        deserialized.ClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.ClassVos_BsonByteVo.Value.Should().Be(1);
        deserialized.ClassVos_BsonCharVo.Value.Should().Be('2');
        deserialized.ClassVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.ClassVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.ClassVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.ClassVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.ClassVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.ClassVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.ClassVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.ClassVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.ClassVos_BsonIntVo.Value.Should().Be(6);
        deserialized.ClassVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.ClassVos_BsonStringVo.Value.Should().Be("8");

        deserialized.RecordClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.RecordClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.RecordClassVos_BsonByteVo.Value.Should().Be(1);
        deserialized.RecordClassVos_BsonCharVo.Value.Should().Be('2');
        deserialized.RecordClassVos_BsonDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.RecordClassVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.RecordClassVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.RecordClassVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.RecordClassVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.RecordClassVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.RecordClassVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.RecordClassVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.RecordClassVos_BsonIntVo.Value.Should().Be(6);
        deserialized.RecordClassVos_BsonLongVo.Value.Should().Be(7L);

        deserialized.StructVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.StructVos_BsonByteVo.Value.Should().Be(1);
        deserialized.StructVos_BsonCharVo.Value.Should().Be('2');
        deserialized.StructVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.StructVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.StructVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.StructVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.StructVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.StructVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.StructVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.StructVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.StructVos_BsonIntVo.Value.Should().Be(6);
        deserialized.StructVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.StructVos_BsonStringVo.Value.Should().Be("8");

        deserialized.RecordStructVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.RecordStructVos_BsonByteVo.Value.Should().Be(1);
        deserialized.RecordStructVos_BsonCharVo.Value.Should().Be('2');
        deserialized.RecordStructVos_BsonDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.RecordStructVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.RecordStructVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.RecordStructVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.RecordStructVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.RecordStructVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.RecordStructVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.RecordStructVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.RecordStructVos_BsonIntVo.Value.Should().Be(6);
        deserialized.RecordStructVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.RecordStructVos_BsonStringVo.Value.Should().Be("8");
    }
}


public class ComplexBson
{
    public Vogen.IntegrationTests.TestTypes.ClassVos.BsonBoolVo ClassVos_BsonBoolVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.ClassVos.BsonBoolVo.From(true);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo ClassVos_BsonByteVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo.From(1);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo ClassVos_BsonCharVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo.From('2');

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo ClassVos_BsonDateTimeOffsetVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo.From(
            new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo ClassVos_BsonDateTimeVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo ClassVos_BsonDecimalVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo.From(3.33m);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo ClassVos_BsonDoubleVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo.From(4.44d);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo ClassVos_BsonFloatVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo.From(5.55f);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo ClassVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.StructVos.Bar(42,  "Fred"));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo ClassVos_BsonGuidVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo.From(Guid.Empty);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo ClassVos_BsonIntVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo.From(6);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo ClassVos_BsonLongVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo.From(7L);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo ClassVos_BsonStringVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo.From("8");

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonBoolVo RecordClassVos_BsonBoolVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonBoolVo.From(true);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonByteVo RecordClassVos_BsonByteVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonByteVo.From(1);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonCharVo RecordClassVos_BsonCharVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonCharVo.From('2');

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeOffsetVo RecordClassVos_BsonDateTimeOffsetVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
            TimeSpan.Zero));

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeVo RecordClassVos_BsonDateTimeVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDecimalVo RecordClassVos_BsonDecimalVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDecimalVo.From(3.33m);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDoubleVo RecordClassVos_BsonDoubleVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDoubleVo.From(4.44d);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFloatVo RecordClassVos_BsonFloatVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFloatVo.From(5.55f);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFooVo RecordClassVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordClassVos.Bar(42, "Fred"));

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonGuidVo RecordClassVos_BsonGuidVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonGuidVo.From(Guid.Empty);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonIntVo RecordClassVos_BsonIntVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonIntVo.From(6);

    public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonLongVo RecordClassVos_BsonLongVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonLongVo.From(7L);


    public Vogen.IntegrationTests.TestTypes.StructVos.BsonBoolVo StructVos_BsonBoolVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonBoolVo.From(true);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo StructVos_BsonByteVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo.From(1);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo StructVos_BsonCharVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo.From('2');

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo StructVos_BsonDateTimeOffsetVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
            TimeSpan.Zero));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo StructVos_BsonDateTimeVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo StructVos_BsonDecimalVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo.From(3.33m);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo StructVos_BsonDoubleVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo.From(4.44d);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo StructVos_BsonFloatVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo.From(5.55f);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo StructVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.StructVos.Bar(42, "Fred"));

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo StructVos_BsonGuidVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo.From(Guid.Empty);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo StructVos_BsonIntVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo.From(6);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo StructVos_BsonLongVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo.From(7L);

    public Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo StructVos_BsonStringVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo.From("8");

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonBoolVo RecordStructVos_BsonBoolVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonBoolVo.From(true);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonByteVo RecordStructVos_BsonByteVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonByteVo.From(1);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonCharVo RecordStructVos_BsonCharVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonCharVo.From('2');

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeOffsetVo RecordStructVos_BsonDateTimeOffsetVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
            TimeSpan.Zero));

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeVo RecordStructVos_BsonDateTimeVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDecimalVo RecordStructVos_BsonDecimalVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDecimalVo.From(3.33m);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDoubleVo RecordStructVos_BsonDoubleVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDoubleVo.From(4.44d);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFloatVo RecordStructVos_BsonFloatVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFloatVo.From(5.55f);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFooVo RecordStructVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordStructVos.Bar(42, "Fred"));

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonGuidVo RecordStructVos_BsonGuidVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonGuidVo.From(Guid.Empty);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonIntVo RecordStructVos_BsonIntVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonIntVo.From(6);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonLongVo RecordStructVos_BsonLongVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonLongVo.From(7L);

    public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonStringVo RecordStructVos_BsonStringVo { get; set; } =
        Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonStringVo.From("8");
}
