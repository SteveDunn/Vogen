using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/575
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

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOnAllFrameworks();
    }
}