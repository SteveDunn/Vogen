namespace Vogen.Examples.Types
{
    [ValueObject<string>(conversions: Conversions.None)]
    public partial struct StringVo { }

    [ValueObject<string>(conversions: Conversions.None)]
    public partial struct NoConverterStringVo { }

    [ValueObject<string>(conversions: Conversions.TypeConverter)]
    public partial struct NoJsonStringVo { }

    [ValueObject<string>(conversions: Conversions.NewtonsoftJson)]
    public partial struct NewtonsoftJsonStringVo { }

    [ValueObject<string>(conversions: Conversions.SystemTextJson)]
    public partial struct SystemTextJsonStringVo { }

    [ValueObject<string>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public partial struct BothJsonStringVo { }

    [ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
    public partial struct EfCoreStringVo { }

    [ValueObject<string>(conversions: Conversions.DapperTypeHandler)]
    public partial struct DapperStringVo { }

    [ValueObject<string>(conversions: Conversions.LinqToDbValueConverter)]
    public partial struct LinqToDbStringVo { }
}
