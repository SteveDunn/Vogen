using System.Collections;
using Shared;
using Vogen;

namespace SnapshotTests.GenerationTests;

[UsesVerify] 
public class InstanceFieldGenerationTests
{
    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(TestData))]
    public Task GenerationTest_FR(string type, string underlyingType, string instanceValue, string className)
    {
        string declaration = $@"
  [ValueObject(underlyingType: typeof({underlyingType}))]
  [Instance(name: ""Invalid"", value: {instanceValue})]
  {type} {className} {{ }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithLocale("fr")
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }
}

public class TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (string type in _types)
        {
            {
                foreach ((string underlyingType, string instanceValue) in _underlyingTypes)
                {
                    var qualifiedType = "public " + type;
                    yield return new object[]
                        {qualifiedType, underlyingType, instanceValue, CreateClassName(qualifiedType, underlyingType)};

                    qualifiedType = "internal " + type;
                    yield return new object[]
                        {qualifiedType, underlyingType, instanceValue, CreateClassName(qualifiedType, underlyingType)};
                }
            }
        }
    }

    private static string CreateClassName(string type, string underlyingType) =>
        type.Replace(" ", "_") + underlyingType;

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

    // for each of the attributes above, use this underlying type
    private readonly (string underlyingType, string instanceValue)[] _underlyingTypes = new[]
    {
        ("byte", "42"),
        ("char", "'x'"),
        ("double", "123.45d"),
        ("float", "123.45f"),
        ("int", "123"),
        ("System.Guid", "Guid.Empty"),
        ("long", "123l"),
        ("string", """123"""),
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
