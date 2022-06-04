namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial record class CharVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
    public partial record class NoConverterCharVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(char))]
    public partial record class NoJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(char))]
    public partial record class NewtonsoftJsonCharVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial record class SystemTextJsonCharVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial record class BothJsonCharVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(char))]
    public partial record class EfCoreCharVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(char))]
    public partial record class DapperCharVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(char))]
    public partial record class LinqToDbCharVo { }
}
