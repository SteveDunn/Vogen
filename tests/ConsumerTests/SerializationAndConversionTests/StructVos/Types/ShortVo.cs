namespace Vogen.IntegrationTests.TestTypes.StructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
public partial struct ShortVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
public partial struct NoConverterShortVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(short))]
public partial struct NoJsonShortVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(short))]
public partial struct NewtonsoftJsonShortVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(short))]
public partial struct SystemTextJsonShortVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(short))]
public partial struct BothJsonShortVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(short))]
public partial struct EfCoreShortVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(short))]
public partial struct DapperShortVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(short))]
public partial struct LinqToDbShortVo { }