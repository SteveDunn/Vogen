namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial record struct ByteVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial record struct NoConverterByteVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
public partial record struct NoJsonByteVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
public partial record struct NewtonsoftJsonByteVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial record struct SystemTextJsonByteVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial record struct BothJsonByteVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
public partial record struct EfCoreByteVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
public partial record struct DapperByteVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(byte))]
public partial record struct LinqToDbByteVo { }