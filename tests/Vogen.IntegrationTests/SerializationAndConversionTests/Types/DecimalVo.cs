namespace Vogen.IntegrationTests.SerializationAndConversionTests.Types
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial struct DecimalVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial struct NoConverterDecimalVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial struct NoJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial struct NewtonsoftJsonDecimalVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct SystemTextJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct BothJsonDecimalVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial struct EfCoreDecimalVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial struct DapperDecimalVo { }
}
