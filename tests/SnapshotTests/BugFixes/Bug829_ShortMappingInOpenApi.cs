using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/829
public class Bug829_ShortMappingInOpenApi
{
    [Fact]
    public async Task ShortMapping()
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

              namespace N;

              [ValueObject<short>]
              public partial class MyVoShort { }

              [ValueObject<ushort>]
              public partial class MyVoUShort { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net9_0);
    }
}