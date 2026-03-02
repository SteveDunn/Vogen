using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/877
public class Bug877_DuplicatesOnPartials
{
    // Previously, any attribute found with an array as a parameter caused an
    // InvalidCastException in Vogen and stopped the generator.
    [Fact]
    public async Task Test()
    {
        var source = """
                     using System;
                     using System.Diagnostics;
                     using Vogen;

                     [ValueObject(conversions: Conversions.None)]
                     public partial class MyVo;
                     
                     [DebuggerDisplay("any attribute used to trick Vogen into provisionally considering it as a VO")]
                     public partial class MyVo;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOnAllFrameworks();
    }
}