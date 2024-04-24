namespace Vogen.IntegrationTests.TestTypes.ClassVos;

public record struct Bar(int Age, string Name);

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
public partial class FooVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
public partial class NoConverterFooVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
public partial class NoJsonFooVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
public partial class NoJsonFooVoClass { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
public partial class NewtonsoftJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
public partial class NewtonsoftJsonFooVoClass { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial class SystemTextJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial class BothJsonFooVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
public partial class BothJsonFooVoClass { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Bar))]
public partial class EfCoreFooVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Bar))]
public partial class DapperFooVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Bar))]
public partial class LinqToDbFooVo { }