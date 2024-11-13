namespace Vogen.IntegrationTests.TestTypes.StructVos;

[ValueObject<bool>(conversions: Conversions.MessagePack)]
public partial struct MessagePackBoolVo;

[ValueObject<float>(conversions: Conversions.MessagePack)]
public partial struct MessagePackFloatVo;

[ValueObject<byte>(conversions: Conversions.MessagePack)]
public partial struct MessagePackByteVo;

[ValueObject<char>(conversions: Conversions.MessagePack)]
public partial struct MessagePackCharVo;

[ValueObject<DateOnly>(conversions: Conversions.MessagePack)]
public partial struct MessagePackDateOnlyVo;

[ValueObject<DateTimeOffset>(conversions: Conversions.MessagePack)]
public partial struct MessagePackDateTimeOffsetVo;

[ValueObject<DateTime>(conversions: Conversions.MessagePack)]
public partial struct MessagePackDateTimeVo;

[ValueObject<decimal>(conversions: Conversions.MessagePack)]
public partial struct MessagePackDecimalVo;

[ValueObject<double>(conversions: Conversions.MessagePack)]
public partial struct MessagePackDoubleVo;

[ValueObject<Bar>(conversions: Conversions.MessagePack)]
public partial struct MessagePackFooVo;

[ValueObject<Guid>(conversions: Conversions.MessagePack)]
public partial struct MessagePackGuidVo;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial struct MessagePackIntVo;

[ValueObject<long>(conversions: Conversions.MessagePack)]
public partial struct MessagePackLongVo;

[ValueObject<short>(conversions: Conversions.MessagePack)]
public partial struct MessagePackShortVo;

[ValueObject<string>(conversions: Conversions.MessagePack)]
public partial struct MessagePackStringVo;

[ValueObject<TimeOnly>(conversions: Conversions.MessagePack)]
public partial struct MessagePackTimeOnlyVo;