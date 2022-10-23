using System;

namespace Vogen.IntegrationTests.TestTypes.RecordStructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial record struct GuidVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial record struct NoConverterGuidVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Guid))]
    public partial record struct NoJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Guid))]
    public partial record struct NewtonsoftJsonGuidVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial record struct SystemTextJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial record struct BothJsonGuidVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Guid))]
    public partial record struct EfCoreGuidVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Guid))]
    public partial record struct DapperGuidVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Guid))]
    public partial record struct LinqToDbGuidVo { }
}
