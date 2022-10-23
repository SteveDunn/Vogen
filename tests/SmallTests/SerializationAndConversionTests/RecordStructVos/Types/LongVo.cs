namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial record struct LongVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial record struct NoConverterLongVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(long))]
    public partial record struct NoJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(long))]
    public partial record struct NewtonsoftJsonLongVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial record struct SystemTextJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial record struct BothJsonLongVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(long))]
    public partial record struct EfCoreLongVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(long))]
    public partial record struct DapperLongVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(long))]
    public partial record struct LinqToDbLongVo { }
}
