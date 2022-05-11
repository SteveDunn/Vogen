using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.GenerationTests;

[UsesVerify] 
public class GenerationTests
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
            type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType;

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
            "byte",
            "char",
            "bool",
            "System.DateTimeOffset",
            "System.DateTime",
            "decimal",
            "double",
            "float",
            //"FooVo",
            "int",
            "System.Guid",
            "long",
            "short",
            "string",
        };
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationTest(string type, string conversions, string underlyingType, string className)
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
}