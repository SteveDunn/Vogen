namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial record struct CharVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial record struct NoConverterCharVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(char))]
    public partial record struct NoJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(char))]
    public partial record struct NewtonsoftJsonCharVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial record struct SystemTextJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial record struct BothJsonCharVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(char))]
    public partial record struct EfCoreCharVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(char))]
    public partial record struct DapperCharVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(char))]
    public partial record struct LinqToDbCharVo { }
}
