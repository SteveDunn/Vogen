namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
    public partial record struct StringVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
    public partial record struct NoConverterStringVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(string))]
    public partial record struct NoJsonStringVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(string))]
    public partial record struct NewtonsoftJsonStringVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial record struct SystemTextJsonStringVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial record struct BothJsonStringVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
    public partial record struct EfCoreStringVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(string))]
    public partial record struct DapperStringVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(string))]
    public partial record struct LinqToDbStringVo { }
}
