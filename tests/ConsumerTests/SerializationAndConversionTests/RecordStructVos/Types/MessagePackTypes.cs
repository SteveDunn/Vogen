namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject<bool>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackBoolVo;

[ValueObject<float>(conversions: Conversions.MessagePack)]
public partial struct MessagePackFloatVo;

[ValueObject<byte>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackByteVo;

[ValueObject<char>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackCharVo;

[ValueObject<DateOnly>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackDateOnlyVo;

[ValueObject<DateTimeOffset>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackDateTimeOffsetVo;

[ValueObject<DateTime>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackDateTimeVo;

[ValueObject<decimal>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackDecimalVo;

[ValueObject<double>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackDoubleVo;

[ValueObject<Bar>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackFooVo;

[ValueObject<Guid>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackGuidVo;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackIntVo;

[ValueObject<long>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackLongVo;

[ValueObject<short>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackShortVo;

[ValueObject<string>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackStringVo;

[ValueObject<TimeOnly>(conversions: Conversions.MessagePack)]
public partial record struct MessagePackTimeOnlyVo;