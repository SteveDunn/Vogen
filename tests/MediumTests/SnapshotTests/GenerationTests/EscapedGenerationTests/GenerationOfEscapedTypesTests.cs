using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediumTests.DiagnosticsTests;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests.EscapedGenerationTests;

[UsesVerify]
public class GenerationOfEscapedTypesTests
{
    public class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in _types)
            {
                string conversion =
                    "Conversions.TypeConverter | Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.EfCoreValueConverter | Conversions.DapperTypeHandler | Conversions.LinqToDbValueConverter";
                
                foreach (string underlyingType in _underlyingTypes)
                {
                    var qualifiedType = "public " + type;
                    yield return new object[]
                    {
                        qualifiedType, conversion, underlyingType,
                        createClassName(qualifiedType, conversion, underlyingType)
                    };
                }
            }
        }

        private string createClassName(string type, string conversion, string underlyingType) =>
            "escapedTests" + type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType;

        private readonly string[] _types = new[]
        {
            "partial struct",
            "partial class",
            "partial record",
            "partial record struct",
        };

        // for each of the attributes above, use this underlying type
        private readonly string[] _underlyingTypes = new[]
        {
            "double",
            "System.Guid",
            "@record.@struct.@float.@decimal",
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public Task GenerationOfEscapedTypes(string type, string conversions, string underlyingType, string className)
    {
        string declaration = $@"

namespace record.@struct.@float
{{
    public readonly record struct @decimal();
}}


  [ValueObject(conversions: {conversions}, underlyingType: typeof({underlyingType}))]
  {type} {className} {{ }}";
        var source = @"using Vogen;
namespace @class
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }
}