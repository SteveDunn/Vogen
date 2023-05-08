namespace Vogen.IntegrationTests.TestTypes.RecordClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
public partial record class StringVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
public partial record class NoConverterStringVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(string))]
public partial record class NoJsonStringVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(string))]
public partial record class NewtonsoftJsonStringVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(string))]
public partial record class SystemTextJsonStringVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(string))]
public partial record class BothJsonStringVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
public partial record class EfCoreStringVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(string))]
public partial record class DapperStringVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(string))]
public partial record class LinqToDbStringVo { }