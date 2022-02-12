using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Vogen.IntegrationTests.SnapshotTests.BoolVo;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.ByteVo;

[UsesVerify] 
public class ByteVoGenerationTests
{
    [Fact]
    public Task NoConverterByteVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
  public partial struct NoConverterByteVo { }");

    [Fact]
    public Task NoJsonByteVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
    public partial struct NoJsonByteVo { }");
    }
    
    [Fact]
    public Task NewtonsoftJsonByteVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
    public partial struct NewtonsoftJsonByteVo { }");
    }
    
    [Fact]
    public Task SystemTextJsonByteVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial struct SystemTextJsonByteVo { }
");
    }

    [Fact]
    public Task BothJsonByteVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
    public partial struct BothJsonByteVo { }");
    }

    [Fact]
    public Task EfCoreByteVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
    public partial struct EfCoreByteVo { }");
    }

    [Fact]
    public Task DapperByteVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
    public partial struct DapperByteVo { }");
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