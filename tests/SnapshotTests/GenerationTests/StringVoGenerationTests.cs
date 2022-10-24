using Shared;
using Vogen;

namespace SnapshotTests.GenerationTests;

[UsesVerify] 
public class StringVoGenerationTests
{
    [Fact]
    public Task NoConverterStringVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(string))]
  public partial struct NoConverterStringVo { }");

    [Fact]
    public Task NoJsonStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(string))]
    public partial struct NoJsonStringVo { }");
    }
    
    [Fact]
    public Task NewtonsoftJsonStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(string))]
    public partial struct NewtonsoftJsonStringVo { }");
    }
    
    [Fact]
    public Task SystemTextJsonStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial struct SystemTextJsonStringVo { }
");
    }

    [Fact]
    public Task BothJsonStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(string))]
    public partial struct BothJsonStringVo { }");
    }

    [Fact]
    public Task EfCoreStringVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(string))]
    public partial struct EfCoreStringVo { }");
    }

    [Fact]
    public Task DapperStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(string))]
    public partial struct DapperStringVo { }");
    }

    [Fact]
    public Task LinqToDbStringVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(string))]
    public partial struct LinqToDbStringVo { }");
    }

    private static Task RunTest(string declaration)
    {
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}