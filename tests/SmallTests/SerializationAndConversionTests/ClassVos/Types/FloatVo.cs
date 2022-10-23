namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial class FloatVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
    public partial class NoConverterFloatVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(float))]
    public partial class NoJsonFloatVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(float))]
    public partial class NewtonsoftJsonFloatVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial class SystemTextJsonFloatVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
    public partial class SystemTextJsonFloatVo_Treating_numbers_as_string { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial class BothJsonFloatVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(float))]
    public partial class EfCoreFloatVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(float))]
    public partial class DapperFloatVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(float))]
    public partial class LinqToDbFloatVo { }
}
