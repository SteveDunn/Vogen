namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(bool))]
public partial class SsdtBoolVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(byte))]
public partial class SsdtByteVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(char))]
public partial class SsdtCharVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateOnly))]
public partial class SsdtDateOnlyVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTimeOffset))]
public partial class SsdtDateTimeOffsetVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTime))]
public partial class SsdtDateTimeVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(decimal))]
public partial class SsdtDecimalVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(double))]
public partial class SsdtDoubleVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Bar))]
public partial class SsdtFooVo
{
    public static SsdtFooVo Parse(string s) => throw new Exception("todo!");
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Guid))]
public partial class SsdtGuidVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(int))]
public partial class SsdtIntVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(long))]
public partial class SsdtLongVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(short))]
public partial class SsdtShortVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(string))]
public partial class SsdtStringVo
{
}

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(TimeOnly))]
public partial class SsdtTimeOnlyVo
{
}