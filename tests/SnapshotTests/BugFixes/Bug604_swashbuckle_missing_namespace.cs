using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/604
[UsesVerify]
public class Bug604_Swashbuckle_has_missing_namespaces_for_the_vo
{
    [Fact]
    public async Task Test()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]
                     
                     namespace MyLongNamespace;
                     
                     [ValueObject<int>]
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