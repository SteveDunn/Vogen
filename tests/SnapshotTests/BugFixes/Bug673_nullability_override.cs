using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/673
public class Bug673_nullability_override
{
    [Fact]
    public async Task When_deriving_from_a_record_it_matches_the_override_for_the_records_ToString()
    {
        var source = """
                     #nullable enable
                     using System;
                     using Vogen;
                     
                     namespace Foo;
                     
                     internal abstract record B;
                     
                     [ValueObject]
                     internal partial record D : B;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net8_0);
    }
}