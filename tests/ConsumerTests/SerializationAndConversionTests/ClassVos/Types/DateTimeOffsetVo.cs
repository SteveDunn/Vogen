namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
public partial class DateTimeOffsetVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
public partial class NoConverterDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTimeOffset))]
public partial class NoJsonDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTimeOffset))]
public partial class NewtonsoftJsonDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
public partial class SystemTextJsonDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(DateTimeOffset))]
public partial class SsdtDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
public partial class BothJsonDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTimeOffset))]
public partial class EfCoreDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTimeOffset))]
public partial class DapperDateTimeOffsetVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTimeOffset))]
public partial class LinqToDbDateTimeOffsetVo { }