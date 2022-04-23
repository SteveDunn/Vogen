using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.LongVo;

[UsesVerify]
public class LongVoGenerationTests
{
    [Fact]
    public Task NoConverterLongVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(long))]
  public partial struct NoConverterLongVo { }");

    [Fact]
    public Task NoJsonLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(long))]
    public partial struct NoJsonLongVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(long))]
    public partial struct NewtonsoftJsonLongVo { }");
    }

    [Fact]
    public Task SystemTextJsonLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial struct SystemTextJsonLongVo { }
");
    }

    [Fact]
    public Task BothJsonLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(long))]
    public partial struct BothJsonLongVo { }");
    }

    [Fact]
    public Task EfCoreLongVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(long))]
    public partial struct EfCoreLongVo { }");
    }

    [Fact]
    public Task DapperLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(long))]
    public partial struct DapperLongVo { }");
    }

    [Fact]
    public Task LinqToDbLongVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(long))]
    public partial struct LinqToDbLongVo { }");
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