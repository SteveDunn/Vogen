using System.Threading.Tasks;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/595
[UsesVerify]
public class Bug595_AttributesWithArraysBreaksGenerator
{
    [Fact]
    public async Task Test()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject(conversions: Conversions.SystemTextJson)]
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