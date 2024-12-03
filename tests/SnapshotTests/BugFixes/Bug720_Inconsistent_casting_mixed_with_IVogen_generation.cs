using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/720
public class Bug720_Inconsistent_casting_mixed_with_IVogen_generation
{
    [Fact]
    public async Task Works_when_the_static_abstracts_and_implementation_have_same_casting()
    {
        // we say that to primitive is implicit and we say that the static abstract interface matches.
        var source = """
                     using Vogen;
                     using static Vogen.StaticAbstractsGeneration;
                     
                     [assembly: VogenDefaults(
                         toPrimitiveCasting: CastOperator.Implicit,
                         staticAbstractsGeneration: ValueObjectsDeriveFromTheInterface |
                                                    EqualsOperators |
                                                    ExplicitCastFromPrimitive |
                                                    ImplicitCastToPrimitive |
                                                    FactoryMethods)]

                     namespace MyApp;
                     
                     [ValueObject<int>]
                     public readonly partial record struct ToDoItemId;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.AspNetCore8_0);
    }
}