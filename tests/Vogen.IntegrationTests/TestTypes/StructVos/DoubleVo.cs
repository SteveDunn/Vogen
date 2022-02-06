namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial struct DoubleVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial struct NoConverterDoubleVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial struct NoJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial struct NewtonsoftJsonDoubleVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct SystemTextJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct BothJsonDoubleVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial struct EfCoreDoubleVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial struct DapperDoubleVo { }
}
