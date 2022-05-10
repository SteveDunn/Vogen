namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial record struct DoubleVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial record struct NoConverterDoubleVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial record struct NoJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial record struct NewtonsoftJsonDoubleVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial record struct SystemTextJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial record struct BothJsonDoubleVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial record struct EfCoreDoubleVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial record struct DapperDoubleVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(double))]
    public partial record struct LinqToDbDoubleVo { }
}
