namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial record class ShortVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial record class NoConverterShortVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(short))]
    public partial record class NoJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(short))]
    public partial record class NewtonsoftJsonShortVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial record class SystemTextJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial record class BothJsonShortVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(short))]
    public partial record class EfCoreShortVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(short))]
    public partial record class DapperShortVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(short))]
    public partial record class LinqToDbShortVo { }
}
