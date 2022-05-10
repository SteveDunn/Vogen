using System;

namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial record class DateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
    public partial record class NoConverterDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record class NoJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTimeOffset))]
    public partial record class NewtonsoftJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial record class SystemTextJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial record class BothJsonDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record class EfCoreDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTimeOffset))]
    public partial record class DapperDateTimeOffsetVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial record class LinqToDbDateTimeOffsetVo { }
}
