using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests;

[UsesVerify]
public class ShortVoGenerationTests
{
    [Fact]
    public Task NoConverterShortVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(short))]
  public partial struct NoConverterShortVo { }");

    [Fact]
    public Task NoJsonShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(short))]
    public partial struct NoJsonShortVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(short))]
    public partial struct NewtonsoftJsonShortVo { }");
    }

    [Fact]
    public Task SystemTextJsonShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial struct SystemTextJsonShortVo { }
");
    }

    [Fact]
    public Task BothJsonShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(short))]
    public partial struct BothJsonShortVo { }");
    }

    [Fact]
    public Task EfCoreShortVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(short))]
    public partial struct EfCoreShortVo { }");
    }

    [Fact]
    public Task DapperShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(short))]
    public partial struct DapperShortVo { }");
    }

    [Fact]
    public Task LinqToDbShortVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(short))]
    public partial struct LinqToDbShortVo { }");
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

        return Verifier.Verify(output).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }
}