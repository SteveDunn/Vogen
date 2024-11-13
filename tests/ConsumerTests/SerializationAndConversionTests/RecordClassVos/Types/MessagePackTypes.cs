namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject<bool>(conversions: Conversions.MessagePack)]
public partial record class MessagePackBoolVo;

[ValueObject<float>(conversions: Conversions.MessagePack)]
public partial struct MessagePackFloatVo;

[ValueObject<byte>(conversions: Conversions.MessagePack)]
public partial record class MessagePackByteVo;

[ValueObject<char>(conversions: Conversions.MessagePack)]
public partial record class MessagePackCharVo;

[ValueObject<DateOnly>(conversions: Conversions.MessagePack)]
public partial record class MessagePackDateOnlyVo;

[ValueObject<DateTimeOffset>(conversions: Conversions.MessagePack)]
public partial record class MessagePackDateTimeOffsetVo;

[ValueObject<DateTime>(conversions: Conversions.MessagePack)]
public partial record class MessagePackDateTimeVo;

[ValueObject<decimal>(conversions: Conversions.MessagePack)]
public partial record class MessagePackDecimalVo;

[ValueObject<double>(conversions: Conversions.MessagePack)]
public partial record class MessagePackDoubleVo;

[ValueObject<Bar>(conversions: Conversions.MessagePack)]
public partial record class MessagePackFooVo;

[ValueObject<Guid>(conversions: Conversions.MessagePack)]
public partial record class MessagePackGuidVo;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial record class MessagePackIntVo;

[ValueObject<long>(conversions: Conversions.MessagePack)]
public partial record class MessagePackLongVo;

[ValueObject<short>(conversions: Conversions.MessagePack)]
public partial record class MessagePackShortVo;

[ValueObject<string>(conversions: Conversions.MessagePack)]
public partial record class MessagePackStringVo;

[ValueObject<TimeOnly>(conversions: Conversions.MessagePack)]
public partial record class MessagePackTimeOnlyVo;