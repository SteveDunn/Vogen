namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
public partial class StringVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
public partial class NoConverterStringVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(string))]
public partial class NoJsonStringVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(string))]
public partial class NewtonsoftJsonStringVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(string))]
public partial class SystemTextJsonStringVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(string))]
public partial class BothJsonStringVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
public partial class EfCoreStringVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(string))]
public partial class DapperStringVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(string))]
public partial class LinqToDbStringVo { }