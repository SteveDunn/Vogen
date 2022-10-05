using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests;

[UsesVerify]
public class IntVoGenerationTests
{
    [Fact]
    public Task NoConverterIntVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
  public partial struct NoConverterIntVo { }");

    [Fact]
    public Task NoJsonIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(int))]
    public partial struct NoJsonIntVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(int))]
    public partial struct NewtonsoftJsonIntVo { }");
    }

    [Fact]
    public Task SystemTextJsonIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial struct SystemTextJsonIntVo { }
");
    }

    [Fact]
    public Task BothJsonIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(int))]
    public partial struct BothJsonIntVo { }");
    }

    [Fact]
    public Task EfCoreIntVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(int))]
    public partial struct EfCoreIntVo { }");
    }

    [Fact]
    public Task DapperIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(int))]
    public partial struct DapperIntVo { }");
    }

    [Fact]
    public Task LinqToDbIntVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(int))]
    public partial struct LinqToDbIntVo { }");
    }

    private static Task RunTest(string declaration)
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