using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.DecimalVo;

[UsesVerify]
public class DecimalVoGenerationTests
{
    [Fact]
    public Task NoConverterDecimalVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(decimal))]
  public partial struct NoConverterDecimalVo { }");

    [Fact]
    public Task NoJsonDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(decimal))]
    public partial struct NoJsonDecimalVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(decimal))]
    public partial struct NewtonsoftJsonDecimalVo { }");
    }

    [Fact]
    public Task SystemTextJsonDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial struct SystemTextJsonDecimalVo { }
");
    }

    [Fact]
    public Task BothJsonDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(decimal))]
    public partial struct BothJsonDecimalVo { }");
    }

    [Fact]
    public Task EfCoreDecimalVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(decimal))]
    public partial struct EfCoreDecimalVo { }");
    }

    [Fact]
    public Task DapperDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(decimal))]
    public partial struct DapperDecimalVo { }");
    }

    [Fact]
    public Task LinqToDbDecimalVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(decimal))]
    public partial struct LinqToDbDecimalVo { }");
    }

    private Task RunTest(string declaration)
    {
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}