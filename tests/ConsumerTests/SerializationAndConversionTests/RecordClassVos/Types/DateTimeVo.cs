namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
public partial record class DateTimeVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
public partial record class NoConverterDateTimeVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTime))]
public partial record class NoJsonDateTimeVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTime))]
public partial record class NewtonsoftJsonDateTimeVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
public partial record class SystemTextJsonDateTimeVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
public partial record class BothJsonDateTimeVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTime))]
public partial record class EfCoreDateTimeVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
public partial record class DapperDateTimeVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTime))]
public partial record class LinqToDbDateTimeVo { }