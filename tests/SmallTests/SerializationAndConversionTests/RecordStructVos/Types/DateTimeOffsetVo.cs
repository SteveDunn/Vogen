using System;

namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial record struct DateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial record struct NoConverterDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record struct NoJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTimeOffset))]
    public partial record struct NewtonsoftJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial record struct SystemTextJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial record struct BothJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record struct EfCoreDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTimeOffset))]
    public partial record struct DapperDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record struct LinqToDbDateTimeOffsetVo { }
}
