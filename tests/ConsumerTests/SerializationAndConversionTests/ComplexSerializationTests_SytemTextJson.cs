#nullable disable
using System.Text.Json;
using Vogen.IntegrationTests.TestTypes;

// ReSharper disable FunctionComplexityOverflow

namespace MediumTests.SerializationAndConversionTests;

public partial class ComplexSerializationTests
{
    public class ComplexSystemTextJson
    {
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonBoolVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonByteVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonCharVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDateTimeVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDecimalVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDoubleVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonFloatVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonFooVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonFooVo.From(new Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonGuidVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonIntVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonLongVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonStringVo Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SystemTextJsonStringVo.From("8");
        
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_ServiceStackTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("9");

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonBoolVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonByteVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonCharVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDateTimeVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDecimalVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDoubleVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonFloatVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonFooVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonFooVo.From(new Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonGuidVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonIntVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonLongVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SystemTextJsonLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonBoolVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonByteVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonCharVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDateTimeVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDecimalVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDoubleVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonFloatVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonFooVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonFooVo.From(new Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonGuidVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonIntVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonLongVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonStringVo Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SystemTextJsonStringVo.From("8");

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonBoolVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonByteVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonCharVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDateTimeVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDecimalVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDoubleVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonFloatVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonFooVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonFooVo.From(new Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonGuidVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonIntVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonLongVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonStringVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SystemTextJsonStringVo.From("8");
    }

    [Fact]
    public void SystemTextJson_CanSerializeAndDeserialize()
    {
        var complex = new ComplexSystemTextJson();

        string serialized = JsonSerializer.Serialize(complex);
        ComplexSystemTextJson deserialized = JsonSerializer.Deserialize<ComplexSystemTextJson>(serialized);

        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SystemTextJsonStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SystemTextJsonLongVo.Value.Should().Be(7L);

        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SystemTextJsonStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SystemTextJsonStringVo.Value.Should().Be("8");

    }
}