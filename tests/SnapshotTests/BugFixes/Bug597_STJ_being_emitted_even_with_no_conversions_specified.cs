using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/597
public class Bug597_STJ_being_emitted_even_with_no_conversions_specified
{
    [Fact]
    public async Task Omitted_if_stj_package_not_referenced()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject(conversions: Conversions.None)]
                     public partial struct Vo
                     {
                     }


                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ExcludeSystemTextJsonNugetPackage()
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net4_6_1);
    }
}