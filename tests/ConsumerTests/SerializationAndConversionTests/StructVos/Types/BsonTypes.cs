namespace Vogen.IntegrationTests.TestTypes.StructVos;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(bool))]
public partial struct BsonBoolVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(byte))]
public partial struct BsonByteVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(char))]
public partial struct BsonCharVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateOnly))]
public partial struct BsonDateOnlyVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTimeOffset))]
public partial struct BsonDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTime))]
public partial struct BsonDateTimeVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(decimal))]
public partial struct BsonDecimalVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(float))]
public partial struct BsonFloatVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(double))]
public partial struct BsonDoubleVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Bar))]
public partial struct BsonFooVo
{
    public static BsonFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Guid))]
public partial struct BsonGuidVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(int))]
public partial struct BsonIntVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(long))]
public partial struct BsonLongVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(short))]
public partial struct BsonShortVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(string))]
public partial struct BsonStringVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(TimeOnly))]
public partial struct BsonTimeOnlyVo
{
}