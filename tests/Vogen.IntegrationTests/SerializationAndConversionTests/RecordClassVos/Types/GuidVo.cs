using System;

namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial record class GuidVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
    public partial record class NoConverterGuidVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Guid))]
    public partial record class NoJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Guid))]
    public partial record class NewtonsoftJsonGuidVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial record class SystemTextJsonGuidVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial record class BothJsonGuidVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Guid))]
    public partial record class EfCoreGuidVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Guid))]
    public partial record class DapperGuidVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Guid))]
    public partial record class LinqToDbGuidVo { }
}
