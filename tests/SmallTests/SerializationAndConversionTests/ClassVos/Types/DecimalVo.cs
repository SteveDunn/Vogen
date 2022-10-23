namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
    public partial class DecimalVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
    public partial class NoConverterDecimalVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(decimal))]
    public partial class NoJsonDecimalVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(decimal))]
    public partial class NewtonsoftJsonDecimalVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial class SystemTextJsonDecimalVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(decimal), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
    public partial class SystemTextJsonDecimalVo_Treating_number_as_string { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial class BothJsonDecimalVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(decimal))]
    public partial class EfCoreDecimalVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(decimal))]
    public partial class DapperDecimalVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(decimal))]
    public partial class LinqToDbDecimalVo { }
}
