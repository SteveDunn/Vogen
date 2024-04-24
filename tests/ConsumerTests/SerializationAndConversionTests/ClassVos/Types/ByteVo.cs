namespace Vogen.IntegrationTests.TestTypes.ClassVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial class ByteVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial class NoConverterByteVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
public partial class NoJsonByteVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
public partial class NewtonsoftJsonByteVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial class SystemTextJsonByteVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class SystemTextJsonByteVo_Treating_numbers_as_string { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial class BothJsonByteVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
public partial class EfCoreByteVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
public partial class DapperByteVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(byte))]
public partial class LinqToDbByteVo { }