using System;

namespace Vogen.IntegrationTests.SerializationAndConversionTests.Types
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
    public partial struct DateTimeVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
    public partial struct NoConverterDateTimeVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTime))]
    public partial struct NoJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTime))]
    public partial struct NewtonsoftJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial struct SystemTextJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial struct BothJsonDateTimeVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTime))]
    public partial struct EfCoreDateTimeVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
    public partial struct DapperDateTimeVo { }
}
