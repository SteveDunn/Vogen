namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
    public partial struct IntVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
    public partial struct NoConverterIntVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(int))]
    public partial struct NoJsonIntVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(int))]
    public partial struct NewtonsoftJsonIntVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial struct SystemTextJsonIntVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial struct BothJsonIntVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(int))]
    public partial struct EfCoreIntVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(int))]
    public partial struct DapperIntVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(int))]
    public partial struct LinqToDbIntVo { }
}
