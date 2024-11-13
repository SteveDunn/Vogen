#nullable disable

using MessagePack;
using ServiceStack.Text;
using Vogen.IntegrationTests.TestTypes;
// ReSharper disable InconsistentNaming

// ReSharper disable FunctionComplexityOverflow

namespace MediumTests.SerializationAndConversionTests;

public partial class ComplexSerializationTests
{
    public class ComplexServiceStackDotText
    {
        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtBoolVo.From(true);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtByteVo.From(1);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtCharVo.From('2');

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeVo
            Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDecimalVo
            Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDecimalVo.From(3.33m);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDoubleVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtDoubleVo.From(4.44d);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtFooVo.From(new Bar(42, "Fred"));

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtGuidVo.From(Guid.Empty);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtIntVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtIntVo.From(6);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_SsdtStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("8");

        public Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo Vogen_IntegrationTests_TestTypes_ClassVos_ServiceStackTextJsonStringVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.ClassVos.SsdtStringVo.From("9");

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtBoolVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtBoolVo.From(true);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtByteVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtByteVo.From(1);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtCharVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtCharVo.From('2');

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDecimalVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDecimalVo.From(3.33m);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDoubleVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtDoubleVo.From(4.44d);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFloatVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFooVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtFooVo.From(new Bar(42, "Fred"));

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtGuidVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtGuidVo.From(Guid.Empty);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtIntVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtIntVo.From(6);

        public Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtLongVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.RecordClassVos.SsdtLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtBoolVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtBoolVo.From(true);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtByteVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtByteVo.From(1);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtCharVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtCharVo.From('2');

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeVo
            Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDecimalVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtDecimalVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.StructVos.SsdtDecimalVo.From(3.33m);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtDoubleVo
            Vogen_IntegrationTests_TestTypes_StructVos_SsdtDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtDoubleVo.From(4.44d);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtFloatVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtFooVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtFooVo.From(new Bar(42, "Fred"));

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtGuidVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtGuidVo.From(Guid.Empty);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtIntVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtIntVo.From(6);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtLongVo Vogen_IntegrationTests_TestTypes_StructVos_SsdtLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.StructVos.SsdtStringVo
            Vogen_IntegrationTests_TestTypes_StructVos_SsdtStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.SsdtStringVo.From("8");

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtBoolVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtBoolVo.From(true);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtByteVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtByteVo.From(1);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtCharVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtCharVo.From('2');

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDecimalVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDecimalVo.From(3.33m);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDoubleVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtDoubleVo.From(4.44d);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFloatVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFloatVo.From(5.55f);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFooVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtFooVo.From(new Bar(42, "Fred"));

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtGuidVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtGuidVo.From(Guid.Empty);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtIntVo Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtIntVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtIntVo.From(6);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtLongVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtLongVo.From(7L);

        public Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtStringVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.SsdtStringVo.From("8");
    }

    [MessagePackObject]
    public class ComplexMessagePack
    {
        [Key(0)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackBoolVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackBoolVo.From(true);

        [Key(1)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackByteVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackByteVo.From(1);

        [Key(2)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackCharVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackCharVo.From('2');

        [Key(3)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        [Key(4)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        [Key(5)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDecimalVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDecimalVo.From(3.33m);

        [Key(6)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDoubleVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDoubleVo.From(4.44d);

        [Key(7)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFloatVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFloatVo.From(5.55f);

        [Key(8)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFooVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFooVo.From(new Bar(42, "Fred"));

        [Key(9)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackGuidVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackGuidVo.From(Guid.Empty);

        [Key(10)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackIntVo Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackIntVo
        {
            get;
            set;
        } = Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackIntVo.From(6);

        [Key(11)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackLongVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackLongVo.From(7L);

        [Key(12)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackStringVo
            Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackStringVo.From("8");

        [Key(13)]
        public Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackStringVo
            Vogen_IntegrationTests_TestTypes_ClassVos_ServiceStackTextJsonStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackStringVo.From("9");

        [Key(14)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackBoolVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackBoolVo.From(true);

        [Key(15)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackByteVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackByteVo.From(1);

        [Key(16)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackCharVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackCharVo.From('2');

        [Key(17)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59,
                999, TimeSpan.Zero));

        [Key(18)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        [Key(19)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDecimalVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDecimalVo.From(3.33m);

        [Key(20)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDoubleVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDoubleVo.From(4.44d);

        [Key(21)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFloatVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFloatVo.From(5.55f);

        [Key(22)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFooVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFooVo.From(new Bar(42, "Fred"));

        [Key(23)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackGuidVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackGuidVo.From(Guid.Empty);

        [Key(24)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackIntVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackIntVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackIntVo.From(6);

        [Key(25)]
        public Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackLongVo
            Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackLongVo.From(7L);

        [Key(26)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackBoolVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackBoolVo.From(true);

        [Key(27)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackByteVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackByteVo.From(1);

        [Key(28)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackCharVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackCharVo.From('2');

        [Key(29)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999,
                TimeSpan.Zero));

        [Key(30)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        [Key(31)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDecimalVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDecimalVo.From(3.33m);

        [Key(32)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDoubleVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDoubleVo.From(4.44d);

        [Key(33)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFloatVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFloatVo.From(5.55f);

        [Key(34)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFooVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFooVo.From(new Bar(42, "Fred"));

        [Key(35)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackGuidVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackGuidVo.From(Guid.Empty);

        [Key(36)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackIntVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackIntVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackIntVo.From(6);

        [Key(37)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackLongVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackLongVo.From(7L);

        [Key(38)]
        public Vogen.IntegrationTests.TestTypes.StructVos.MessagePackStringVo
            Vogen_IntegrationTests_TestTypes_StructVos_MessagePackStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.StructVos.MessagePackStringVo.From("8");

        [Key(39)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackBoolVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackBoolVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackBoolVo.From(true);

        [Key(40)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackByteVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackByteVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackByteVo.From(1);

        [Key(41)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackCharVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackCharVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackCharVo.From('2');

        [Key(42)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeOffsetVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDateTimeOffsetVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeOffsetVo.From(new DateTimeOffset(2020, 12, 13, 23, 59, 59,
                999, TimeSpan.Zero));

        [Key(43)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDateTimeVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeVo.From(new DateTime(2020, 12, 13, 23, 59, 59, 999,
                DateTimeKind.Utc));

        [Key(44)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDecimalVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDecimalVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDecimalVo.From(3.33m);

        [Key(45)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDoubleVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDoubleVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDoubleVo.From(4.44d);

        [Key(46)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFloatVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackFloatVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFloatVo.From(5.55f);

        [Key(47)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFooVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackFooVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFooVo.From(new Bar(42, "Fred"));

        [Key(48)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackGuidVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackGuidVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackGuidVo.From(Guid.Empty);

        [Key(49)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackIntVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackIntVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackIntVo.From(6);

        [Key(50)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackLongVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackLongVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackLongVo.From(7L);

        [Key(51)]
        public Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackStringVo
            Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackStringVo { get; set; } =
            Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackStringVo.From("8");
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
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_SsdtDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
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
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_SsdtDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
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
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_SsdtDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
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
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_SsdtDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
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

    [Fact]
    public void MessagePack_CanSerializeAndDeserialize()
    {
        ConsumerTests.SerializationAndConversionTests.FooFormatter fooFormatter = new();


        var customResolver = MessagePack.Resolvers.CompositeResolver.Create(
            [
                fooFormatter,
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackBoolVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFloatVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackByteVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackCharVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateOnlyVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeOffsetVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDateTimeVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDecimalVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackDoubleVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackFooVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackGuidVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackIntVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackLongVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackShortVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackStringVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.ClassVos.MessagePackTimeOnlyVoMessagePackFormatter(),

                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackBoolVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFloatVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackByteVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackCharVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateOnlyVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeOffsetVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDateTimeVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDecimalVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackDoubleVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackFooVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackGuidVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackIntVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackLongVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackShortVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackStringVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordClassVos.MessagePackTimeOnlyVoMessagePackFormatter(),

                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackBoolVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFloatVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackByteVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackCharVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateOnlyVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeOffsetVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDateTimeVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDecimalVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackDoubleVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackFooVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackGuidVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackIntVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackLongVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackShortVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackStringVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.StructVos.MessagePackTimeOnlyVoMessagePackFormatter(),

                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackBoolVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFloatVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackByteVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackCharVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateOnlyVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeOffsetVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDateTimeVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDecimalVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackDoubleVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackFooVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackGuidVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackIntVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackLongVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackShortVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackStringVoMessagePackFormatter(),
                new Vogen.IntegrationTests.TestTypes.RecordStructVos.MessagePackTimeOnlyVoMessagePackFormatter()
            ],
            [MessagePack.Resolvers.StandardResolver.Instance]
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(customResolver);

        var complex = new ComplexMessagePack();

        var mp = MessagePackSerializer.Serialize(complex, options);

        var deserialized = MessagePackSerializer.Deserialize<ComplexMessagePack>(mp, options);

        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_ClassVos_MessagePackStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999, DateTimeKind.Utc));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordClassVos_MessagePackLongVo.Value.Should().Be(7L);

        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_StructVos_MessagePackStringVo.Value.Should().Be("8");

        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackBoolVo.Value.Should().Be(true);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackByteVo.Value.Should().Be(1);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackCharVo.Value.Should().Be('2');
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDateTimeOffsetVo.Value.Should()
            .Be(new DateTimeOffset(2020, 12, 13, 23, 59, 59, 999, TimeSpan.Zero));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDateTimeVo.Value.Should()
            .Be(new DateTime(2020, 12, 13, 23, 59, 59, 999));
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDecimalVo.Value.Should().Be(3.33m);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackDoubleVo.Value.Should().Be(4.44d);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackFloatVo.Value.Should().Be(5.55f);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackFooVo.Value.Age.Should().Be(42);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackFooVo.Value.Name.Should().Be("Fred");
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackGuidVo.Value.Should().Be(Guid.Empty);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackIntVo.Value.Should().Be(6);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackLongVo.Value.Should().Be(7L);
        deserialized.Vogen_IntegrationTests_TestTypes_RecordStructVos_MessagePackStringVo.Value.Should().Be("8");
    }
}