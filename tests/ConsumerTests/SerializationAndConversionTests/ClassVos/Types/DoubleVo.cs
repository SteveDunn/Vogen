namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
public partial class DoubleVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
public partial class NoConverterDoubleVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
public partial class NoJsonDoubleVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
public partial class NewtonsoftJsonDoubleVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
public partial class SystemTextJsonDoubleVo { }

[ValueObject(conversions: Conversions.ServiceStackDotText, underlyingType: typeof(double))]
public partial class SsdtJsonDoubleVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class SystemTextJsonDoubleVo_number_as_string { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
public partial class BothJsonDoubleVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
public partial class EfCoreDoubleVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
public partial class DapperDoubleVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(double))]
public partial class LinqToDbDoubleVo { }