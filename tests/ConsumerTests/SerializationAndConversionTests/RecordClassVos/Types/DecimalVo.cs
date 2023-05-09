namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
public partial record class DecimalVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
public partial record class NoConverterDecimalVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(decimal))]
public partial record class NoJsonDecimalVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(decimal))]
public partial record class NewtonsoftJsonDecimalVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(decimal))]
public partial record class SystemTextJsonDecimalVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(decimal))]
public partial record class BothJsonDecimalVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(decimal))]
public partial record class EfCoreDecimalVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(decimal))]
public partial record class DapperDecimalVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(decimal))]
public partial record class LinqToDbDecimalVo { }