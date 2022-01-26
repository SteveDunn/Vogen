using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.CharVo;

[UsesVerify] 
public class CharVoGenerationTests
{
    [Fact]
    public Task NoConverterCharVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(char))]
  public partial struct NoConverterCharVo { }");

    [Fact]
    public Task NoJsonCharVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(char))]
    public partial struct NoJsonCharVo { }");
    }
    
    [Fact]
    public Task NewtonsoftJsonCharVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(char))]
    public partial struct NewtonsoftJsonCharVo { }");
    }
    
    [Fact]
    public Task SystemTextJsonCharVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial struct SystemTextJsonCharVo { }
");
    }

    [Fact]
    public Task BothJsonCharVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(char))]
    public partial struct BothJsonCharVo { }");
    }

    [Fact]
    public Task EfCoreCharVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(char))]
    public partial struct EfCoreCharVo { }");
    }

    [Fact]
    public Task DapperCharVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(char))]
    public partial struct DapperCharVo { }");
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