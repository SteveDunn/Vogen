using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.BoolVo;

[UsesVerify] 
public class BoolVoGenerationTests
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in _types)
            {
                foreach (string conversion in _conversions)
                {
                    foreach (string underlyingType in _underlyingTypes)
                    {
                        yield return new object[] { type, conversion, underlyingType, createClassName(type, conversion, underlyingType) };
                    }
                }
            }
        }

        private string createClassName(string type, string conversion, string underlyingType) => 
            type.Replace(" ", "_") + conversion.Replace(".", "_") + underlyingType;

        private readonly string[] _types = new[]
        {
            "public partial struct",
            "public partial class",
            "public partial record struct",
            "public partial record class",
            "public readonly partial struct",
            "public readonly partial record struct"
        };

        // for each of the types above, create classes for each one of these attributes
        private readonly string[] _conversions = new[]
        {
            "Conversions.None",
            "Conversions.TypeConverter",
            "Conversions.NewtonsoftJson",
            "Conversions.SystemTextJson",
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson",
            "Conversions.EfCoreValueConverter",
            "Conversions.DapperTypeHandler",
            "Conversions.LinqToDbValueConverter",
        };

        // for each of the attributes above, use this underlying type
        private readonly string[] _underlyingTypes = new[]
        {
            "bool",
            "string",
            "int",
            "char",
            "double",
            "float"
        };
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task NoConverterBoolVo_Test(string type, string conversions, string underlyingType, string className)
    {
        string declaration = $@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof({underlyingType}))]
  {type} {className} {{ }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        VerifySettings settings = new VerifySettings();
        settings.UseFileName(className);
        return Verifier.Verify(output, settings).UseDirectory("Snapshots");
    }


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

    private Task RunTest(string declaration)
    {
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output, null).UseDirectory("Snapshots");
    }
}