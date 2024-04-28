using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/575
[UsesVerify]
public class Bug575_AttributesWithArraysBreaksGenerator
{
    // Previously, any attribute found with an array as a parameter caused an
    // InvalidCastException in Vogen and stopped the generator.
    [Fact]
    public async Task Test()
    {
        var source = """

                     using System;
                     using Vogen;
                     
                     public class Test<T> : Attribute { }

                     [Test<byte[]>]
                     public class Test
                     {
                        Test() => Vo.From(123);
                     }

                     [ValueObject]
                     public partial struct Vo
                     {
                     }
                     
                     
                     """;

        await RunTest(source);
    }


    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
        
}