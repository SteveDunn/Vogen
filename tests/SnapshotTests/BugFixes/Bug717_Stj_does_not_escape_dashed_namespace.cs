using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/717
public class Bug717_Stj_does_not_escape_dashed_namespace
{
    [Fact]
    public async Task Test()
    {
        var source = """

                     using System;
                     using Vogen;
                     using System.Text.Json;
                     
                     namespace My_Namespace;
                     
                     [ValueObject(typeof(Guid))]
                     public partial struct Vo;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithAssemblyName("MY-PROJECT")
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}