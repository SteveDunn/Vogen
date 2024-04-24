namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
public partial class CharVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
public partial class NoConverterCharVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(char))]
public partial class NoJsonCharVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(char))]
public partial class NewtonsoftJsonCharVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(char))]
public partial class SystemTextJsonCharVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(char))]
public partial class BothJsonCharVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(char))]
public partial class EfCoreCharVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(char))]
public partial class DapperCharVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(char))]
public partial class LinqToDbCharVo { }