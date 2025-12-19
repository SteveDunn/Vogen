using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/810
public class Bug810Tests
{
    [Fact]
    public async Task Test()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.SystemTextJson | Conversions.TypeConverter | Conversions.Orleans)]

                     namespace N;

                     [ValueObject<string>(conversions: Conversions.SystemTextJson | Conversions.TypeConverter)]
                     internal partial class MyVo;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}