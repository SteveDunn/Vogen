using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

public class Bug833_STJ_being_emitted_even_when_not_referenced
{
    // See https://github.com/SteveDunn/Vogen/issues/833
    
    [Fact]
    public async Task Omitted_if_stj_package_not_referenced()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject]
                     public partial struct Vo
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ExcludeSystemTextJsonNugetPackage()
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net4_8);
    }
}