using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.FooVo;

[UsesVerify]
public class FooVoGenerationTests
{
    [Fact]
    public Task NoConverterFooVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(Foo))]
  public partial struct NoConverterFooVo { }");

    [Fact]
    public Task NoJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Foo))]
    public partial struct NoJsonFooVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Foo))]
    public partial struct NewtonsoftJsonFooVo { }");
    }

    [Fact]
    public Task SystemTextJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Foo))]
    public partial struct SystemTextJsonFooVo { }
");
    }

    [Fact]
    public Task BothJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Foo))]
    public partial struct BothJsonFooVo { }");
    }

    [Fact]
    public Task EfCoreFooVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Foo))]
    public partial struct EfCoreFooVo { }");
    }

    [Fact]
    public Task DapperFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Foo))]
    public partial struct DapperFooVo { }");
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