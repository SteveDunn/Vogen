namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial record class FloatVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial record class NoConverterFloatVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(float))]
    public partial record class NoJsonFloatVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(float))]
    public partial record class NewtonsoftJsonFloatVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial record class SystemTextJsonFloatVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial record class BothJsonFloatVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(float))]
    public partial record class EfCoreFloatVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(float))]
    public partial record class DapperFloatVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(float))]
    public partial record class LinqToDbFloatVo { }
}
