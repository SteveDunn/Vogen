namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
public partial class GuidVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
public partial class NoConverterGuidVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Guid))]
public partial class NoJsonGuidVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Guid))]
public partial class NewtonsoftJsonGuidVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Guid))]
public partial class SystemTextJsonGuidVo { }

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(Guid))]
public partial class SsdtGuidVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Guid))]
public partial class BothJsonGuidVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Guid))]
public partial class EfCoreGuidVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Guid))]
public partial class DapperGuidVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Guid))]
public partial class LinqToDbGuidVo { }