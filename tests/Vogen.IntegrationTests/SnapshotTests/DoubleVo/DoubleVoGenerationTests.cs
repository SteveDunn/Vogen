using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.DoubleVo;

[UsesVerify]
public class DoubleVoGenerationTests
{
    [Fact]
    public Task NoConverterDoubleVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(double))]
  public partial struct NoConverterDoubleVo { }");

    [Fact]
    public Task NoJsonDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(double))]
    public partial struct NoJsonDoubleVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(double))]
    public partial struct NewtonsoftJsonDoubleVo { }");
    }

    [Fact]
    public Task SystemTextJsonDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct SystemTextJsonDoubleVo { }
");
    }

    [Fact]
    public Task BothJsonDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(double))]
    public partial struct BothJsonDoubleVo { }");
    }

    [Fact]
    public Task EfCoreDoubleVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(double))]
    public partial struct EfCoreDoubleVo { }");
    }

    [Fact]
    public Task DapperDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(double))]
    public partial struct DapperDoubleVo { }");
    }

    [Fact]
    public Task LinqToDbDoubleVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(double))]
    public partial struct LinqToDbDoubleVo { }");
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