using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/575
public class Bug896_ByteMappingInOpenApi
{
    [Fact]
    public async Task ByteMapping()
    {
        var source =
            $$"""
              using System;
              using Vogen;

              [assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

              namespace N;

              [ValueObject<byte>]
              public partial class MyVoByte { }

              [ValueObject<char>]
              public partial class MyVoChar { }

              [ValueObject<char>]
              public partial class MyVoShort { }
              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net9_0);
    }
}