namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
    public partial record struct DecimalVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
    public partial record struct NoConverterDecimalVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(decimal))]
    public partial record struct NoJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(decimal))]
    public partial record struct NewtonsoftJsonDecimalVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial record struct SystemTextJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial record struct BothJsonDecimalVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(decimal))]
    public partial record struct EfCoreDecimalVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(decimal))]
    public partial record struct DapperDecimalVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(decimal))]
    public partial record struct LinqToDbDecimalVo { }
}
