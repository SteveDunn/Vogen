namespace Vogen.StringegrationTests.NewTests.Types
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
    public partial struct StringVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
    public partial struct NoConverterStringVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(string))]
    public partial struct NoJsonStringVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(string))]
    public partial struct NewtonsoftJsonStringVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial struct SystemTextJsonStringVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial struct BothJsonStringVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
    public partial struct EfCoreStringVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(string))]
    public partial struct DapperStringVo { }
}
