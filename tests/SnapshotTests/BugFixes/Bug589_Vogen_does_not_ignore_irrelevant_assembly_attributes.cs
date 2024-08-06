using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/589
public class Bug589_Vogen_does_not_ignore_irrelevant_assembly_attributes
{
    [Fact]
    public async Task Test()
    {
        var source = """

                     using System;
                     using Vogen;

                     [assembly: Test(1, 2, 3, 4, 5)]
                     [assembly: VogenDefaults(conversions: Conversions.Default | Conversions.LinqToDbValueConverter)]

                     [AttributeUsage(AttributeTargets.Assembly)]
                     #pragma warning disable CS9113 // Parameter 'Values' is unread
                     public sealed class TestAttribute(params int[] Values) : Attribute 
                     {
                     }

                     [ValueObject]
                     public partial struct Vo
                     {
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }
}