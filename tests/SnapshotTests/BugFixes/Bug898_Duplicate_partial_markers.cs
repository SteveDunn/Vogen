using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

public class Bug898_Duplicate_partial_markers
{
    [Fact]
    public async Task Handles_partial_markers_for_bson()
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
    [Fact]
    public async Task Handles_partial_markers_for_efcore()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace Whatever;

                     [ValueObject<int>]
                     public partial class CustomerId;

                     [ValueObject<int>]
                     public partial class UserId;

                     [ValueObject<int>]
                     public partial class AnotherUserId;

                     [EfCoreConverter<UserId>]
                     [EfCoreConverter<CustomerId>]
                     internal partial class EfCoreConverters;

                     [EfCoreConverter<AnotherUserId>]
                     internal partial class EfCoreConverters;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    
}