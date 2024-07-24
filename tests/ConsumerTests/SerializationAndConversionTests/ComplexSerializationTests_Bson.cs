#nullable disable
using System.Text.Json;
// ReSharper disable FunctionComplexityOverflow

namespace MediumTests.SerializationAndConversionTests;

public partial class ComplexSerializationTests
{
    public class ComplexBson
    {
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonBoolVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonByteVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonCharVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonDateTimeVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonDecimalVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonDoubleVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonFloatVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonFooVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.ClassVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonGuidVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonIntVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonLongVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.ClassVos.BsonStringVo Vogen_IntegrationTests_TestTypes_ClassVos_BsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.BsonStringVo.From("8");
        
        //public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_ServiceStackTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("9");

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonBoolVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonByteVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonCharVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDecimalVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDoubleVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFloatVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFooVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordClassVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonGuidVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonIntVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonLongVo Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.BsonLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.StructVos.BsonBoolVo Vogen_IntegrationTests_TestTypes_StructVos_BsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo Vogen_IntegrationTests_TestTypes_StructVos_BsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo Vogen_IntegrationTests_TestTypes_StructVos_BsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_StructVos_BsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo Vogen_IntegrationTests_TestTypes_StructVos_BsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo Vogen_IntegrationTests_TestTypes_StructVos_BsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo Vogen_IntegrationTests_TestTypes_StructVos_BsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo Vogen_IntegrationTests_TestTypes_StructVos_BsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo Vogen_IntegrationTests_TestTypes_StructVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.StructVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo Vogen_IntegrationTests_TestTypes_StructVos_BsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo Vogen_IntegrationTests_TestTypes_StructVos_BsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo Vogen_IntegrationTests_TestTypes_StructVos_BsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo Vogen_IntegrationTests_TestTypes_StructVos_BsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.BsonStringVo.From("8");

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonBoolVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonByteVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonCharVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDecimalVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDoubleVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFloatVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFooVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordStructVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonGuidVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonIntVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonLongVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonStringVo Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.BsonStringVo.From("8");
    }

    [Fact]
    public void Bson_CanSerializeAndDeserialize()
    {
        var complex = new ComplexBson();

        string serialized = JsonSerializer.Serialize(complex);
        ComplexBson deserialized = JsonSerializer.Deserialize<ComplexBson>(serialized);

        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_BsonStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_BsonLongVo.Value.Should().Be(7L);

        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_BsonStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_BsonStringVo.Value.Should().Be("8");

    }
}