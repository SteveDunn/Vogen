using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/610
public class Bug610_Inconsistent_casting
{
    [Fact]
    public async Task Setting_implicit_casting_to_primitive_in_global_config_should_not_write_a_primitive_cast_to_wrapper()
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
    public async Task Setting_implicit_casting_to_primitive_in_value_object_config_should_write_a_primitive_cast()
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