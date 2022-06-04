namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial record struct ShortVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial record struct NoConverterShortVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(short))]
    public partial record struct NoJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(short))]
    public partial record struct NewtonsoftJsonShortVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial record struct SystemTextJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial record struct BothJsonShortVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(short))]
    public partial record struct EfCoreShortVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(short))]
    public partial record struct DapperShortVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(short))]
    public partial record struct LinqToDbShortVo { }
}
