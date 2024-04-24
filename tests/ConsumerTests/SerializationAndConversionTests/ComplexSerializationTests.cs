#nullable disable

using ServiceStack.Text;
// ReSharper disable FunctionComplexityOverflow

namespace MediumTests.SerializationAndConversionTests;

public partial class ComplexSerializationTests
{
    public class ComplexServiceStackDotText
    {
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDecimalVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDoubleVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFooVo.From(new Vogen.IntegrationTests.TestTypes.ClassVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("8");
        
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_ServiceStackTextJsonStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("9");

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDecimalVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDoubleVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFloatVo.From(5.55f);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordClassVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDecimalVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDoubleVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtFooVo.From(new Vogen.IntegrationTests.TestTypes.StructVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtStringVo.From("8");

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtBoolVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtBoolVo.From(true);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtByteVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtByteVo.From(1);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtCharVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtCharVo.From('2');
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeOffsetVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeOffsetVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDecimalVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDecimalVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDecimalVo.From(3.33m);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDoubleVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDoubleVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDoubleVo.From(4.44d);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFloatVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFooVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFooVo.From(new Vogen.IntegrationTests.TestTypes.RecordStructVos.Bar(42, "Fred"));
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtGuidVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtGuidVo.From(Guid.Empty);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtIntVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtIntVo.From(6);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtLongVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtLongVo.From(7L);
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtStringVo { get; set; } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtStringVo.From("8");
    }

    [Fact]
    public void Ssdt_CanSerializeAndDeserialize()
    {
        var complex = new ComplexServiceStackDotText();

        string serialized = JsonSerializer.SerializeToString(complex);
        ComplexServiceStackDotText deserialized = JsonSerializer.DeserializeFromString<ComplexServiceStackDotText>(serialized);

        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtLongVo.Value.Should().Be(7L);

        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeOffsetVo.Value.Should().Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeVo.Value.Should().Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtStringVo.Value.Should().Be("8");
    }
}