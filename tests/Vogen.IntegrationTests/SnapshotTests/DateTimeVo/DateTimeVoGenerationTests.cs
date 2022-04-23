using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.DateTimeVo;

[UsesVerify]
public class DateTimeVoGenerationTests
{
    [Fact]
    public Task NoConverterDateTimeVo_Test() =>
        RunTest(@"
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(DateTime))]
  public partial struct NoConverterDateTimeVo { }");

    [Fact]
    public Task NoJsonDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(DateTime))]
    public partial struct NoJsonDateTimeVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(DateTime))]
    public partial struct NewtonsoftJsonDateTimeVo { }");
    }

    [Fact]
    public Task SystemTextJsonDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial struct SystemTextJsonDateTimeVo { }
");
    }

    [Fact]
    public Task BothJsonDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(DateTime))]
    public partial struct BothJsonDateTimeVo { }");
    }

    [Fact]
    public Task EfCoreDateTimeVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(DateTime))]
    public partial struct EfCoreDateTimeVo { }");
    }

    [Fact]
    public Task DapperDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(DateTime))]
    public partial struct DapperDateTimeVo { }");
    }

    [Fact]
    public Task LinqToDbDateTimeVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(DateTime))]
    public partial struct LinqToDbDateTimeVo { }");
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