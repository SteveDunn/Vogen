using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BsonSerializationGeneration;

// contrib: An idea place to start a new feature. Write a new test for the feature here to get it working, then
// add more tests. Move these tests if there are several of them, and it makes sense to group them.

public class BsonSerializationGenerationTests
{
    [Fact]
    public async Task Writes_bson_serializers()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     [assembly: VogenDefaults(conversions: Conversions.Bson)]

                     namespace Whatever;
                     [ValueObject<int>]
                     public partial struct Age;

                     [ValueObject<string>]
                     public partial struct Name;
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Escapes_namespaces()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     [assembly: VogenDefaults(conversions: Conversions.Bson)]

                     namespace @int;
                     [ValueObject<string>]
                     public partial struct Name;
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Generates_from_a_marker()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     [assembly: VogenDefaults(conversions: Conversions.None)]

                     namespace N;
                     
                     [ValueObject<string>]
                     public partial struct Name;

                     [ValueObject<int>]
                     public partial struct Age;
                     
                     [BsonSerializer<Name>]
                     [BsonSerializer<Age>]
                     public partial class MyMarkers;
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net8_0);
    }
}