using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Shared;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit;

namespace LargeTests.SnapshotTests.GenerationTests.EscapedGenerationTests;

[UsesVerify]
public class GenerationOfEscapedTypesTests
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
                        var qualifiedType = "public " + type;
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType) };

                        qualifiedType = "internal " + type;
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType) };
                    }
                }
            }
        }

        private string createClassName(string type, string conversion, string underlyingType) =>
            "escapedTests" + type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType;

        private readonly string[] _types = new[]
        {
            "partial struct",
            "readonly partial struct",

            "partial class",
            "sealed partial class",

            "partial record struct",
            "readonly partial record struct",

            "partial record class",
            "sealed partial record class",

            "partial record",
            "sealed partial record",
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
            "double",
            "System.Guid",
            "string",
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

        // return new SnapshotRunner<ValueObjectGenerator>()
        //     .WithSource(source)
        //     .WithLocale("fr")
        //     .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
        //     .RunOnAllFrameworks();




        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        VerifySettings settings = new VerifySettings();
        
        // settings.AutoVerify();
        // settings.UseFileName(SnapshotUtils.ShortenForFilename(className));

        settings.UseFileName(className);
        return Verifier.Verify(output, settings).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName());
    }
}