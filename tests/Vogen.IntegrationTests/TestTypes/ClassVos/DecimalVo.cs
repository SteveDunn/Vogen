namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial class DecimalVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial class NoConverterDecimalVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial class NoJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial class NewtonsoftJsonDecimalVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial class SystemTextJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial class BothJsonDecimalVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial class EfCoreDecimalVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial class DapperDecimalVo { }
}
