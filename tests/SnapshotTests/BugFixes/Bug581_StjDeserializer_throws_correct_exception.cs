using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/581
[UsesVerify]
public class Bug581_StjDeserializer_throws_correct_exception
{
    // The System.Text.Json converter called Guid.Parse, which in turn threw a NullReferenceException.
    // It should now use Guid.TryParse and return an STJ exception.
    // This test just verifies that the correct code is generated. The consumer tests verify the runtime
    // behaviour.
    [Fact]
    public async Task Test()
    {
        var source = """

                     using System;
                     using Vogen;
                     using System.Text.Json;
                     
                     [ValueObject(typeof(Guid))]
                     public partial struct Vo
                     {
                     }
                     
                     
                     """;

        await RunTest(source);
    }


    private static Task RunTest(string source) =>
        new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
        
}