using System;

namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial struct DateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial struct NoConverterDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTimeOffset))]
    public partial struct NoJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct NewtonsoftJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct SystemTextJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct BothJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial struct EfCoreDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTimeOffset))]
    public partial struct DapperDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial struct LinqToDbDateTimeOffsetVo { }
}
