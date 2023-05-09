namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
public partial record struct DateTimeVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
public partial record struct NoConverterDateTimeVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTime))]
public partial record struct NoJsonDateTimeVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTime))]
public partial record struct NewtonsoftJsonDateTimeVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
public partial record struct SystemTextJsonDateTimeVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
public partial record struct BothJsonDateTimeVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTime))]
public partial record struct EfCoreDateTimeVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
public partial record struct DapperDateTimeVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTime))]
public partial record struct LinqToDbDateTimeVo { }