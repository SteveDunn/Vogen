using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.OrleansGeneration;

public class OrleansGenerationTests
{
    [Fact]
    public async Task Do()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.Orleans)]

                     namespace OrleansTests;

                     [ValueObject<int>]
                     public partial struct Age;

                     [ValueObject<string>]
                     public partial struct Name;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("Microsoft.Orleans.Serialization", "8.2.0", string.Empty))
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }
}