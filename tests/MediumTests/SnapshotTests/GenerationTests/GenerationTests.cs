using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests;

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
                        var qualifiedType = "public " + type;
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType) };
                        
                        qualifiedType = "internal " + type;
                        yield return new object[] { qualifiedType, conversion, underlyingType, createClassName(qualifiedType, conversion, underlyingType) };
                    }
                }
            }
        }

        private string createClassName(string type, string conversion, string underlyingType) => 
            type.Replace(" ", "_") + conversion.Replace(".", "_").Replace("|", "_") + underlyingType;

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
            "Conversions.NewtonsoftJson | Conversions.SystemTextJson",
            "Conversions.EfCoreValueConverter | Conversions.DapperTypeHandler | Conversions.LinqToDbValueConverter",
        };

        // for each of the attributes above, use this underlying type
        private readonly string[] _underlyingTypes = new[]
        {
            "byte",
            "char",
            "System.DateTimeOffset",
            "System.DateTime",
            "decimal",
            "int",
            "System.Guid",
            "long",
            "string",
        };
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(Types))]
    public Task GenerationTests_FR(string type, string conversions, string underlyingType, string className)
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
        settings.UseFileName(TestHelper.ShortenForFilename(className));
#if AUTO_VERIFY
        settings.AutoVerify();
#endif
        return Verifier.Verify(output, settings).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName("fr"));
    }

    [Theory]
    [UseCulture("en-US")]
    [ClassData(typeof(Types))]
    public Task GenerationTests_US(string type, string conversions, string underlyingType, string className)
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
        settings.UseFileName(TestHelper.ShortenForFilename(className));
#if AUTO_VERIFY
        settings.AutoVerify();
#endif
        return Verifier.Verify(output, settings).UseDirectory(SnapshotUtils.GetSnapshotDirectoryName("us"));
    }
}