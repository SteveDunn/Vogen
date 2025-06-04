using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/795
public class Bug795_tuples_with_3_or_more_items_crashes_generator
{
    [Fact]
    public async Task Correct_generates_xml_comments_for_tuples()
    {
        // we say that to primitive is implicit and we say that the static abstract interface matches.
        var source = """
                     using Vogen;
                     
                     namespace MyApp;
                     
                     [ValueObject<(int, string)>]
                     public partial class Farm;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net9_0);
    }

    [Fact]
    public async Task Works_with_two_tuples()
    {
        // we say that to primitive is implicit and we say that the static abstract interface matches.
        var source = """
                     using Vogen;
                     
                     namespace MyApp;
                     
                     [ValueObject]
                     public partial struct FarmId;
                     
                     [ValueObject<string>]
                     public partial struct FarmName;

                     [ValueObject<(FarmId, FarmName)>]
                     public partial class Farm  //or (readonly) struct
                     {
                         public FarmId Id => Value.Item1;
                         public FarmName Name => Value.Item2;
                     
                         private static (FarmId, FarmName) NormalizeInput((FarmId, FarmName) input)
                             => input;
                         private static Validation Validate((FarmId, FarmName) input)
                             => Validation.Ok;
                     }
                     

                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net9_0);
    }
}