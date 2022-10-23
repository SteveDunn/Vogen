namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial record class BoolVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
    public partial record class NoConverterBoolVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(bool))]
    public partial record class NoJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(bool))]
    public partial record class NewtonsoftJsonBoolVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial record class SystemTextJsonBoolVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial record class BothJsonBoolVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(bool))]
    public partial record class EfCoreBoolVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(bool))]
    public partial record class DapperBoolVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(bool))]
    public partial record class LinqToDbBoolVo { }
}
