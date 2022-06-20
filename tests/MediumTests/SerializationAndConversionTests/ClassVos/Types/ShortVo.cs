namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial class ShortVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
    public partial class NoConverterShortVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(short))]
    public partial class NoJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(short))]
    public partial class NewtonsoftJsonShortVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial class SystemTextJsonShortVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial class BothJsonShortVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(short))]
    public partial class EfCoreShortVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(short))]
    public partial class DapperShortVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(short))]
    public partial class LinqToDbShortVo { }
}
