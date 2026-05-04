using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/610
public class Bug610_Inconsistent_casting
{
    [Fact]
    public async Task Global_config_implicit_cast_omits_primitive_cast()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(toPrimitiveCasting: CastOperator.Implicit, conversions: Conversions.None)]
                     
                     namespace MyLongNamespace;
                     
                     [ValueObject<int>(toPrimitiveCasting: CastOperator.Implicit)]
                     public partial struct Vo
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.AspNetCore8_0);
    }

    [Fact]
    public async Task Local_config_implicit_cast_writes_primitive_cast()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace MyLongNamespace;
                     
                     [ValueObject<int>(toPrimitiveCasting: CastOperator.Implicit)]
                     public partial struct Vo
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.AspNetCore8_0);
    }
}