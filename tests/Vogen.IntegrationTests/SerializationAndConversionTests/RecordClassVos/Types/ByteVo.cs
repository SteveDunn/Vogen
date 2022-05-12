namespace Vogen.IntegrationTests.TestTypes.RecordClassVos
{
    [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
    public partial record class ByteVo { }

    [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
    public partial record class NoConverterByteVo { }

    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
    public partial record class NoJsonByteVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
    public partial record class NewtonsoftJsonByteVo { }

    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial record class SystemTextJsonByteVo { }

    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial record class BothJsonByteVo { }

    [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
    public partial record class EfCoreByteVo { }

    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
    public partial record class DapperByteVo { }

    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(byte))]
    public partial record class LinqToDbByteVo { }
}
