namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public partial record class IntVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public partial record class NoConverterIntVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(int))]
public partial record class NoJsonIntVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(int))]
public partial record class NewtonsoftJsonIntVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int))]
public partial record class SystemTextJsonIntVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(int))]
public partial record class BothJsonIntVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(int))]
public partial record class EfCoreIntVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(int))]
public partial record class DapperIntVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(int))]
public partial record class LinqToDbIntVo { }