namespace Vogen.IntegrationTests.TestTypes.StructVos;

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(bool))]
public partial struct SsdtBoolVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(byte))]
public partial struct SsdtByteVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(char))]
public partial struct SsdtCharVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateOnly))]
public partial struct SsdtDateOnlyVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTimeOffset))]
public partial struct SsdtDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTime))]
public partial struct SsdtDateTimeVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(decimal))]
public partial struct SsdtDecimalVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(float))]
public partial struct SsdtFloatVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(double))]
public partial struct SsdtDoubleVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Bar))]
public partial struct SsdtFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Guid))]
public partial struct SsdtGuidVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(int))]
public partial struct SsdtIntVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(long))]
public partial struct SsdtLongVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(short))]
public partial struct SsdtShortVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(string))]
public partial struct SsdtStringVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(TimeOnly))]
public partial struct SsdtTimeOnlyVo
{
}