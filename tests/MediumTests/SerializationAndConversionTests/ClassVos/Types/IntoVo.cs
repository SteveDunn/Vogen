namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
    public partial class IntVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
    public partial class NoConverterIntVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(int))]
    public partial class NoJsonIntVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(int))]
    public partial class NewtonsoftJsonIntVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial class SystemTextJsonIntVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
    public partial class SystemTextJsonIntVo_Treating_numbers_as_string { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial class BothJsonIntVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(int))]
    public partial class EfCoreIntVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(int))]
    public partial class DapperIntVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(int))]
    public partial class LinqToDbIntVo { }
}
