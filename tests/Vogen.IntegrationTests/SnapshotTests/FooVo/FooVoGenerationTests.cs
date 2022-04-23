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
  [ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
  public partial struct NoConverterFooVo { }");

    [Fact]
    public Task NoJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(Bar))]
    public partial struct NoJsonFooVo { }");
    }

    [Fact]
    public Task NewtonsoftJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(Bar))]
    public partial struct NewtonsoftJsonFooVo { }");
    }

    [Fact]
    public Task SystemTextJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(Bar))]
    public partial struct SystemTextJsonFooVo { }
");
    }

    [Fact]
    public Task BothJsonFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(Bar))]
    public partial struct BothJsonFooVo { }");
    }

    [Fact]
    public Task EfCoreFooVo_Test()
    {
        return RunTest(@"
        [ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(Bar))]
    public partial struct EfCoreFooVo { }");
    }

    [Fact]
    public Task DapperFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(Bar))]
    public partial struct DapperFooVo { }");
    }

    [Fact]
    public Task LinqToDbFooVo_Test()
    {
        return RunTest(@"
    [ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(Bar))]
    public partial struct LinqToDbFooVo { }");
    }

    private Task RunTest(string declaration)
    {
        var source = @"using System;
using Vogen;
namespace Whatever
{

public record struct Bar(int Age, string Name);

" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}