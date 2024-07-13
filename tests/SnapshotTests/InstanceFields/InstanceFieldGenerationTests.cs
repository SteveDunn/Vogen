using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.InstanceFields;

public class InstanceFieldGenerationTests
{
    [Fact]
    public Task Instances_can_be_booleans()
    {
        var source = """
using Vogen;

namespace Whatever;

[ValueObject(typeof(bool))]
[Instance("Invalid", false)]
public partial struct BooleanThing
{
}
""";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Instances_can_be_newed_up_in_the_wrapper_itself()
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public partial class MyVo
                       {
                           public static readonly MyVo Unspecified = new MyVo(-1);
                       }

                       """;
        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Instance_names_can_have_reserved_keywords()
    {
        var source = @"using Vogen;

namespace Whatever;

[ValueObject]
[Instance(name: ""@class"", value: 42)]
[Instance(name: ""@event"", value: 69)]
public partial struct CustomerId
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid(""must be greater than zero"");
    }
}
";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Theory]
    [UseCulture("fr-FR")]
    [ClassData(typeof(TestData))]
    public Task GenerationTest_FR(string type, string underlyingType, string instanceValue,
        string className) => Run(
        type,
        underlyingType,
        instanceValue,
        className,
        "fr");

    [Theory]
    [ClassData(typeof(TestData))]
    public Task GenerationTest(string type, string underlyingType, string instanceValue,
        string className) => Run(
        type,
        underlyingType,
        instanceValue,
        className,
        "");

    private Task Run(string type, string underlyingType, string instanceValue, string className, string locale)
    {
        string declaration = $@"
  [ValueObject(underlyingType: typeof({underlyingType}))]
  [Instance(name: ""MyValue"", value: {instanceValue})]
  {type} {className} {{}}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithLocale(locale)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(className)))
            .RunOnAllFrameworks();
    }
}

public class TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (string type in Factory.TypeVariations)
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

    // for each of the attributes above, use this underlying type
    private readonly (string underlyingType, string instanceValue)[] _underlyingTypes = new[]
    {
        ("byte", "42"),
        ("char", "'x'"),
        ("double", "123.45d"),
        ("float", "123.45f"),
        ("int", "123"),
        ("long", "123L"),
        ("string", """123"""),
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
