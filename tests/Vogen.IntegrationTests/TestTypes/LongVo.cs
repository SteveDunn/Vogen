namespace Vogen.IntegrationTests.TestTypes
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial struct LongVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial struct NoConverterLongVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(long))]
    public partial struct NoJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(long))]
    public partial struct NewtonsoftJsonLongVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial struct SystemTextJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial struct BothJsonLongVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(long))]
    public partial struct EfCoreLongVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(long))]
    public partial struct DapperLongVo { }
}
