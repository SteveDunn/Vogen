namespace Vogen.Examples.Types
{
    [ValueObject<float>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public readonly partial struct Celsius { }

    [ValueObject<float>(conversions: Conversions.None)]
    public partial struct FloatVo { }

    [ValueObject<float>(conversions: Conversions.None)]
    public partial struct NoConverterFloatVo { }

    [ValueObject<float>(conversions: Conversions.TypeConverter)]
    public partial struct NoJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.NewtonsoftJson)]
    public partial struct NewtonsoftJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.SystemTextJson)]
    public partial struct SystemTextJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public partial struct BothJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.EfCoreValueConverter)]
    public partial struct EfCoreFloatVo { }

    [ValueObject<float>(conversions: Conversions.DapperTypeHandler)]
    public partial struct DapperFloatVo { }

    [ValueObject<float>(conversions: Conversions.LinqToDbValueConverter)]
    public partial struct LinqToDbFloatVo { }
}
