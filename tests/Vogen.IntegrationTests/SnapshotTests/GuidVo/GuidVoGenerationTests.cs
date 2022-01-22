using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.GuidVo;

[UsesVerify]
public class GuidVoGenerationTests
{
    [Fact]
    public Task NoConverterGuidVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(Guid))]
  public partial struct NoConverterGuidVo { }");

    [Fact]
    public Task NoJsonGuidVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Guid))]
    public partial struct NoJsonGuidVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonGuidVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Guid))]
    public partial struct NewtonsoftJsonGuidVo { }");
    }

    [Fact]
    public Task SystemTextJsonGuidVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial struct SystemTextJsonGuidVo { }
");
    }

    [Fact]
    public Task BothJsonGuidVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Guid))]
    public partial struct BothJsonGuidVo { }");
    }

    [Fact]
    public Task EfCoreGuidVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Guid))]
    public partial struct EfCoreGuidVo { }");
    }

    [Fact]
    public Task DapperGuidVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Guid))]
    public partial struct DapperGuidVo { }");
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