namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(bool))]
public partial record class BsonBoolVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(byte))]
public partial record class BsonByteVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(char))]
public partial record class BsonCharVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateOnly))]
public partial record class BsonDateOnlyVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTimeOffset))]
public partial record class BsonDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(DateTime))]
public partial record class BsonDateTimeVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(decimal))]
public partial record class BsonDecimalVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(double))]
public partial record class BsonDoubleVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(float))]
public partial record class BsonFloatVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Bar))]
public partial record class BsonFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(Guid))]
public partial record class BsonGuidVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(int))]
public partial record class BsonIntVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(long))]
public partial record class BsonLongVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(short))]
public partial record class BsonShortVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(string))]
public partial record class BsonStringVo
{
}

[ValueObject(conversions: Conversions.Bson, underlyingType: typeof(TimeOnly))]
public partial record class BsonTimeOnlyVo
{
}