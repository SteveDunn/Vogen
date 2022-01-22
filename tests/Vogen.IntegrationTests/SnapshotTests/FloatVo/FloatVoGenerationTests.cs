using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.FloatVo;

[UsesVerify]
public class FloatVoGenerationTests
{
    [Fact]
    public Task NoConverterFloatVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(float))]
  public partial struct NoConverterFloatVo { }");

    [Fact]
    public Task NoJsonFloatVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(float))]
    public partial struct NoJsonFloatVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonFloatVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(float))]
    public partial struct NewtonsoftJsonFloatVo { }");
    }

    [Fact]
    public Task SystemTextJsonFloatVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial struct SystemTextJsonFloatVo { }
");
    }

    [Fact]
    public Task BothJsonFloatVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
    public partial struct BothJsonFloatVo { }");
    }

    [Fact]
    public Task EfCoreFloatVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(float))]
    public partial struct EfCoreFloatVo { }");
    }

    [Fact]
    public Task DapperFloatVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(float))]
    public partial struct DapperFloatVo { }");
    }

    private Task RunTest(string declaration)
    {
        var source = @"using System;
using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}