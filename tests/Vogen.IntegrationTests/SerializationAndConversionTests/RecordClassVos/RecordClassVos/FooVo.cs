namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    public record struct Bar(int Age, string Name);

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
    public partial record class FooVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
    public partial record class NoConverterFooVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
    public partial record class NoJsonFooVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
    public partial record class NoJsonFooVoClass { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
    public partial record class NewtonsoftJsonFooVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
    public partial record class NewtonsoftJsonFooVoClass { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Bar))]
    public partial record class SystemTextJsonFooVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
    public partial record class BothJsonFooVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
    public partial record class BothJsonFooVoClass { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Bar))]
    public partial record class EfCoreFooVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Bar))]
    public partial record class DapperFooVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Bar))]
    public partial record class LinqToDbFooVo { }
}
