#if NET6_0_OR_GREATER

using System;

namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateOnly))]
    public partial class DateOnlyVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateOnly))]
    public partial class NoConverterDateOnlyVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateOnly))]
    public partial class NoJsonDateOnlyVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateOnly))]
    public partial class NewtonsoftJsonDateOnlyVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateOnly))]
    public partial class SystemTextJsonDateOnlyVo { }

    [ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateOnly))]
    public partial class SsdtDateOnlyVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateOnly))]
    public partial class BothJsonDateOnlyVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateOnly))]
    public partial class EfCoreDateOnlyVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateOnly))]
    public partial class DapperDateOnlyVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateOnly))]
    public partial class LinqToDbDateOnlyVo { }
}

#endif
