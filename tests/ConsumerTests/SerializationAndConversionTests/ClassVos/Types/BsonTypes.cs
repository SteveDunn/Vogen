namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(bool))]
public partial class BsonBoolVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(float))]
public partial class BsonFloatVo;


[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(byte))]
public partial class BsonByteVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(char))]
public partial class BsonCharVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateOnly))]
// ReSharper disable once UnusedType.Global - The C# driver doesn't yet support this
public partial class BsonDateOnlyVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTimeOffset))]
public partial class BsonDateTimeOffsetVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTime))]
public partial class BsonDateTimeVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(decimal))]
public partial class BsonDecimalVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(double))]
public partial class BsonDoubleVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Bar))]
public partial class BsonFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Guid))]
public partial class BsonGuidVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(int))]
public partial class BsonIntVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(long))]
public partial class BsonLongVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(short))]
public partial class BsonShortVo;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(string))]
public partial class BsonStringVo;

// The C# driver doesn't yet support this
[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(TimeOnly))]
public partial class BsonTimeOnlyVo;