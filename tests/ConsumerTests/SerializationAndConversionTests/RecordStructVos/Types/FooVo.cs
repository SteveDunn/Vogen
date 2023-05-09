namespace Vogen.IntegrationTests.TestTypes.RecordStructVos;

public record struct Bar(int Age, string Name);

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
public partial record struct FooVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
public partial record struct NoConverterFooVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
public partial record struct NoJsonFooVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
public partial record struct NoJsonFooVoClass { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
public partial record struct NewtonsoftJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
public partial record struct NewtonsoftJsonFooVoClass { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial record struct SystemTextJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial record struct BothJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial record struct BothJsonFooVoClass { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Bar))]
public partial record struct EfCoreFooVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Bar))]
public partial record struct DapperFooVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Bar))]
public partial record struct LinqToDbFooVo { }