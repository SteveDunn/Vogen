namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public partial record struct IntVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public partial record struct NoConverterIntVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(int))]
public partial record struct NoJsonIntVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(int))]
public partial record struct NewtonsoftJsonIntVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int))]
public partial record struct SystemTextJsonIntVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(int))]
public partial record struct BothJsonIntVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(int))]
public partial record struct EfCoreIntVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(int))]
public partial record struct DapperIntVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(int))]
public partial record struct LinqToDbIntVo { }