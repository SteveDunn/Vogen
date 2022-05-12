namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial record class LongVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
    public partial record class NoConverterLongVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(long))]
    public partial record class NoJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(long))]
    public partial record class NewtonsoftJsonLongVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial record class SystemTextJsonLongVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial record class BothJsonLongVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(long))]
    public partial record class EfCoreLongVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(long))]
    public partial record class DapperLongVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(long))]
    public partial record class LinqToDbLongVo { }
}
