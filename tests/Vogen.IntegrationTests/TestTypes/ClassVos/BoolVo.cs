namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial class BoolVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial class NoConverterBoolVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(bool))]
    public partial class NoJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(bool))]
    public partial class NewtonsoftJsonBoolVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial class SystemTextJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial class BothJsonBoolVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(bool))]
    public partial class EfCoreBoolVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(bool))]
    public partial class DapperBoolVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(bool))]
    public partial class LinqToDbBoolVo { }
}
