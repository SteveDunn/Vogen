using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.DateTimeOffsetVo;

[UsesVerify]
public class DateTimeOffsetVoGenerationTests
{
    [Fact]
    public Task NoConverterDateTimeOffsetVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTimeOffset))]
  public partial struct NoConverterDateTimeOffsetVo { }");

    [Fact]
    public Task NoJsonDateTimeOffsetVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTimeOffset))]
    public partial struct NoJsonDateTimeOffsetVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonDateTimeOffsetVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct NewtonsoftJsonDateTimeOffsetVo { }");
    }

    [Fact]
    public Task SystemTextJsonDateTimeOffsetVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct SystemTextJsonDateTimeOffsetVo { }
");
    }

    [Fact]
    public Task BothJsonDateTimeOffsetVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTimeOffset))]
    public partial struct BothJsonDateTimeOffsetVo { }");
    }

    [Fact]
    public Task EfCoreDateTimeOffsetVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTimeOffset))]
    public partial struct EfCoreDateTimeOffsetVo { }");
    }

    [Fact]
    public Task DapperDateTimeOffsetVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTimeOffset))]
    public partial struct DapperDateTimeOffsetVo { }");
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