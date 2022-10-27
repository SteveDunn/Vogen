using System;

namespace Vogen.IntegrationTests.TestTypes.StructVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial struct GuidVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial struct NoConverterGuidVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Guid))]
    public partial struct NoJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Guid))]
    public partial struct NewtonsoftJsonGuidVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial struct SystemTextJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial struct BothJsonGuidVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Guid))]
    public partial struct EfCoreGuidVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Guid))]
    public partial struct DapperGuidVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Guid))]
    public partial struct LinqToDbGuidVo { }
}
