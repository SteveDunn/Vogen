namespace Vogen.Examples.Types
{
    [ValueObject(conversions: Conversions.None)]
    public partial struct IntVo { }

    [ValueObject<int>(conversions: Conversions.None)]
    public partial struct IntGenericVo { }

    [ValueObject<int>(conversions: Conversions.None)]
    public partial struct NoConverterIntVo { }

    [ValueObject(conversions: Conversions.TypeConverter)]
    public partial struct NoJsonIntVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson)]
    public partial struct NewtonsoftJsonIntVo { }

    [ValueObject(conversions: Conversions.SystemTextJson)]
    public partial struct SystemTextJsonIntVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public partial struct BothJsonIntVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter)]
    public partial struct EfCoreIntVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler)]
    public partial struct DapperIntVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter)]
    public partial struct LinqToDbIntVo { }
}
