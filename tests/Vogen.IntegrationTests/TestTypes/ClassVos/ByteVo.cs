namespace Vogen.IntegrationTests.TestTypes.ClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
    public partial class ByteVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
    public partial class NoConverterByteVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
    public partial class NoJsonByteVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
    public partial class NewtonsoftJsonByteVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial class SystemTextJsonByteVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial class BothJsonByteVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
    public partial class EfCoreByteVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
    public partial class DapperByteVo { }
}
