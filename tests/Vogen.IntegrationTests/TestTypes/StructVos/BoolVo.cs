namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial struct BoolVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial struct NoConverterBoolVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(bool))]
    public partial struct NoJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(bool))]
    public partial struct NewtonsoftJsonBoolVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial struct SystemTextJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial struct BothJsonBoolVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(bool))]
    public partial struct EfCoreBoolVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(bool))]
    public partial struct DapperBoolVo { }
}
