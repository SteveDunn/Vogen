namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
public partial record struct BoolVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
public partial record struct NoConverterBoolVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(bool))]
public partial record struct NoJsonBoolVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(bool))]
public partial record struct NewtonsoftJsonBoolVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(bool))]
public partial record struct SystemTextJsonBoolVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(bool))]
public partial record struct BothJsonBoolVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(bool))]
public partial record struct EfCoreBoolVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(bool))]
public partial record struct DapperBoolVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(bool))]
public partial record struct LinqToDbBoolVo { }