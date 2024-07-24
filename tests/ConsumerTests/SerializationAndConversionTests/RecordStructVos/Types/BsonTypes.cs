namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(bool))]
public partial record struct BsonBoolVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(byte))]
public partial record struct BsonByteVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(char))]
public partial record struct BsonCharVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateOnly))]
public partial record struct BsonDateOnlyVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTimeOffset))]
public partial record struct BsonDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTime))]
public partial record struct BsonDateTimeVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(float))]
public partial record struct BsonFloatVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(decimal))]
public partial record struct BsonDecimalVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(double))]
public partial record struct BsonDoubleVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Bar))]
public partial record struct BsonFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Guid))]
public partial record struct BsonGuidVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(int))]
public partial record struct BsonIntVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(long))]
public partial record struct BsonLongVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(short))]
public partial record struct BsonShortVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(string))]
public partial record struct BsonStringVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(TimeOnly))]
public partial record struct BsonTimeOnlyVo
{
}