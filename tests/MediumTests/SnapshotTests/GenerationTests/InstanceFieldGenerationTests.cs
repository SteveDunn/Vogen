using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.GenerationTests;

public class C
{
    public C(byte b)
    {
    }

    void f() { new C((byte)'a'); }
}

[ValueObject(underlyingType: typeof(byte))]
[Instance(name: "Invalid", value: (byte)1)]
public partial struct xxx_ByteVo
{
    // public static xxx_ByteVo Invalid = new xxx_ByteVo('x');
    // public xxx_ByteVo()
    // {
    //     
    // }
}

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
                        yield return new object[] { qualifiedType, underlyingType, instanceValue, createClassName(qualifiedType, underlyingType) };
                        
                        qualifiedType = "internal " + type;
                        yield return new object[] { qualifiedType, underlyingType, instanceValue, createClassName(qualifiedType, underlyingType) };
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
        
        //var a = new DateTimeOffset(2020, 12, 13, 12, 13, 42, TimeSpan.MinValue)

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
