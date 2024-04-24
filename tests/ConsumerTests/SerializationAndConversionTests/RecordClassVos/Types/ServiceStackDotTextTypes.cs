namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(bool))]
public partial record class SsdtBoolVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(byte))]
public partial record class SsdtByteVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(char))]
public partial record class SsdtCharVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateOnly))]
public partial record class SsdtDateOnlyVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTimeOffset))]
public partial record class SsdtDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTime))]
public partial record class SsdtDateTimeVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(decimal))]
public partial record class SsdtDecimalVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(double))]
public partial record class SsdtDoubleVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(float))]
public partial record class SsdtFloatVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Bar))]
public partial record class SsdtFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Guid))]
public partial record class SsdtGuidVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(int))]
public partial record class SsdtIntVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(long))]
public partial record class SsdtLongVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(short))]
public partial record class SsdtShortVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(string))]
public partial record class SsdtStringVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(TimeOnly))]
public partial record class SsdtTimeOnlyVo
{
}