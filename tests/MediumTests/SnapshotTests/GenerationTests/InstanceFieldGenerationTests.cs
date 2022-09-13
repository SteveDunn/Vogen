using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests;

[UsesVerify] 
public class InstanceFieldGenerationTests
{
    [Fact]
    public Task When_value_is_text_and_cannot_be_converted_to_underlying()
    {
        string declaration = $@"using System;
  [ValueObject(underlyingType: typeof(float))]
  [Instance(name: ""Invalid"", value: ""1.23x"")]
  public partial class MyInstanceTests {{ }}";
        var source = @"using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using var _ = new AssertionScope();
        diagnostics.Should().HaveCount(1);
        diagnostics.Single().GetMessage().Should().Be("MyInstanceTests cannot be converted. Instance value named Invalid has an attribute with a 'System.String' of '1.23x' which cannot be converted to the underlying type of 'System.Single' - Input string was not in a correct format.");

        diagnostics.Single().Id.Should().Be("VOG023");

        return Task.CompletedTask;
    }

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

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        VerifySettings settings = new VerifySettings();
        settings.UseFileName(TestHelper.ShortenForFilename(className));
        //settings.AutoVerify();
        return Verifier.Verify(output, settings).UseDirectory("Snapshots-fr");
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
                        {qualifiedType, underlyingType, instanceValue, createClassName(qualifiedType, underlyingType)};

                    qualifiedType = "internal " + type;
                    yield return new object[]
                        {qualifiedType, underlyingType, instanceValue, createClassName(qualifiedType, underlyingType)};
                }
            }
        }
    }

    private string createClassName(string type, string underlyingType) =>
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
