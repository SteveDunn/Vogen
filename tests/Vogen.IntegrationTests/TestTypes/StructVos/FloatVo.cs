namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial struct FloatVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial struct NoConverterFloatVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(float))]
    public partial struct NoJsonFloatVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(float))]
    public partial struct NewtonsoftJsonFloatVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial struct SystemTextJsonFloatVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial struct BothJsonFloatVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(float))]
    public partial struct EfCoreFloatVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(float))]
    public partial struct DapperFloatVo { }
}
