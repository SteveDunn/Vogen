using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.BoolVo;

[UsesVerify] 
public class BoolVoGenerationTests
{
    [Fact]
    public Task NoConverterBoolVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
  public partial struct NoConverterBoolVo { }");

    [Fact]
    public Task NoJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
    public partial struct NoJsonBoolVo { }");
    }
    
    [Fact]
    public Task NewtonsoftJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
    public partial struct NewtonsoftJsonBoolVo { }");
    }
    
    [Fact]
    public Task SystemTextJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial struct SystemTextJsonBoolVo { }
");
    }

    [Fact]
    public Task BothJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial struct BothJsonBoolVo { }");
    }

    [Fact]
    public Task EfCoreBoolVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
    public partial struct EfCoreBoolVo { }");
    }

    [Fact]
    public Task DapperBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
    public partial struct DapperBoolVo { }");
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