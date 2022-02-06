using System;

namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
    public partial class DateTimeVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
    public partial class NoConverterDateTimeVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTime))]
    public partial class NoJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTime))]
    public partial class NewtonsoftJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial class SystemTextJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial class BothJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTime))]
    public partial class EfCoreDateTimeVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
    public partial class DapperDateTimeVo { }
}
