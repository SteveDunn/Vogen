using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.BoolVo;

[UsesVerify] 
public class BoolVoGenerationTests
{
    [Fact]
    public Task NoConverterBoolVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(bool))]
  public partial struct NoConverterBoolVo { }");

    [Fact]
    public Task NoJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(bool))]
    public partial struct NoJsonBoolVo { }");
    }
    
    [Fact]
    public Task NewtonsoftJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(bool))]
    public partial struct NewtonsoftJsonBoolVo { }");
    }
    
    [Fact]
    public Task SystemTextJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial struct SystemTextJsonBoolVo { }
");
    }

    [Fact]
    public Task BothJsonBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(bool))]
    public partial struct BothJsonBoolVo { }");
    }

    [Fact]
    public Task EfCoreBoolVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(bool))]
    public partial struct EfCoreBoolVo { }");
    }

    [Fact]
    public Task DapperBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(bool))]
    public partial struct DapperBoolVo { }");
    }

    [Fact]
    public Task LinqToDbBoolVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(bool))]
    public partial struct DapperBoolVo { }");
    }

    private Task RunTest(string declaration, VerifySettings? settings = null)
    {
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output, settings).UseDirectory("Snapshots");
    }
}