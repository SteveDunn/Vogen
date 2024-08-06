using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/608
public class Bug608_swashbuckle_mappingTests
{
    [Fact]
    public async Task Used_to_treat_non_primitives_as_objects_but_now_treats_IParsable_as_strings()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]
                     
                     namespace MyLongNamespace;
                     
                     [ValueObject<Guid>]
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