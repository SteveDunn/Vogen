namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
public partial record struct FloatVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
public partial record struct NoConverterFloatVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(float))]
public partial record struct NoJsonFloatVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(float))]
public partial record struct NewtonsoftJsonFloatVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float))]
public partial record struct SystemTextJsonFloatVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
public partial record struct BothJsonFloatVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(float))]
public partial record struct EfCoreFloatVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(float))]
public partial record struct DapperFloatVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(float))]
public partial record struct LinqToDbFloatVo { }