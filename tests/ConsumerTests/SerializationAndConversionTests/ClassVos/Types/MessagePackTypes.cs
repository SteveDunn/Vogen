namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject<bool>(conversions: Conversions.MessagePack)]
public partial class MessagePackBoolVo;

[ValueObject<float>(conversions: Conversions.MessagePack)]
public partial struct MessagePackFloatVo;

[ValueObject<byte>(conversions: Conversions.MessagePack)]
public partial class MessagePackByteVo;

[ValueObject<char>(conversions: Conversions.MessagePack)]
public partial class MessagePackCharVo;

[ValueObject<DateOnly>(conversions: Conversions.MessagePack)]
public partial class MessagePackDateOnlyVo;

[ValueObject<DateTimeOffset>(conversions: Conversions.MessagePack)]
public partial class MessagePackDateTimeOffsetVo;

[ValueObject<DateTime>(conversions: Conversions.MessagePack)]
public partial class MessagePackDateTimeVo;

[ValueObject<decimal>(conversions: Conversions.MessagePack)]
public partial class MessagePackDecimalVo;

[ValueObject<double>(conversions: Conversions.MessagePack)]
public partial class MessagePackDoubleVo;

[ValueObject<Bar>(conversions: Conversions.MessagePack)]
public partial class MessagePackFooVo;

[ValueObject<Guid>(conversions: Conversions.MessagePack)]
public partial class MessagePackGuidVo;

[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial class MessagePackIntVo;

[ValueObject<long>(conversions: Conversions.MessagePack)]
public partial class MessagePackLongVo;

[ValueObject<short>(conversions: Conversions.MessagePack)]
public partial class MessagePackShortVo;

[ValueObject<string>(conversions: Conversions.MessagePack)]
public partial class MessagePackStringVo;

[ValueObject<TimeOnly>(conversions: Conversions.MessagePack)]
public partial class MessagePackTimeOnlyVo;