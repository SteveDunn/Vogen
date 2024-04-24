namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(bool))]
public partial record struct SsdtBoolVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(byte))]
public partial record struct SsdtByteVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(char))]
public partial record struct SsdtCharVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateOnly))]
public partial record struct SsdtDateOnlyVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTimeOffset))]
public partial record struct SsdtDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTime))]
public partial record struct SsdtDateTimeVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(float))]
public partial record struct SsdtFloatVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(decimal))]
public partial record struct SsdtDecimalVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(double))]
public partial record struct SsdtDoubleVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Bar))]
public partial record struct SsdtFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Guid))]
public partial record struct SsdtGuidVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(int))]
public partial record struct SsdtIntVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(long))]
public partial record struct SsdtLongVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(short))]
public partial record struct SsdtShortVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(string))]
public partial record struct SsdtStringVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(TimeOnly))]
public partial record struct SsdtTimeOnlyVo
{
}