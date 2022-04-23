namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial struct CharVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial struct NoConverterCharVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(char))]
    public partial struct NoJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(char))]
    public partial struct NewtonsoftJsonCharVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial struct SystemTextJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial struct BothJsonCharVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(char))]
    public partial struct EfCoreCharVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(char))]
    public partial struct DapperCharVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(char))]
    public partial struct LinqToDbCharVo { }
}
