namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial record class DoubleVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
    public partial record class NoConverterDoubleVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial record class NoJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial record class NewtonsoftJsonDoubleVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial record class SystemTextJsonDoubleVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial record class BothJsonDoubleVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial record class EfCoreDoubleVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial record class DapperDoubleVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(double))]
    public partial record class LinqToDbDoubleVo { }
}
