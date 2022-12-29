#if NET6_0_OR_GREATER

using System;

namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(TimeOnly))]
    public partial class TimeOnlyVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(TimeOnly))]
    public partial class NoConverterTimeOnlyVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(TimeOnly))]
    public partial class NoJsonTimeOnlyVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(TimeOnly))]
    public partial class NewtonsoftJsonTimeOnlyVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(TimeOnly))]
    public partial class SystemTextJsonTimeOnlyVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(TimeOnly))]
    public partial class BothJsonTimeOnlyVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(TimeOnly))]
    public partial class EfCoreTimeOnlyVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(TimeOnly))]
    public partial class DapperTimeOnlyVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(TimeOnly))]
    public partial class LinqToDbTimeOnlyVo { }
}

#endif
