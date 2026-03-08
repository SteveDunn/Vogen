using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

public class Bug898_Duplicate_partial_markers
{
    [Fact]
    public async Task Handles_partial_markers_for_messagepack()
    {
        var source =
            """

              using System;
              using Vogen;

              namespace @double;

              [ValueObject<int>(conversions: Conversions.None)]
              public partial struct MyId;

              [ValueObject<int>(conversions: Conversions.None)]
              public partial struct MyId2;

              [MessagePack<MyId>]
              public partial class MyMarkers;

              [MessagePack<MyId2>]
              public partial class MyMarkers;

              """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("MessagePack", "2.5.187", "lib/netstandard2.0" ))
            .RunOn(TargetFramework.Net8_0);
    }

      [Fact]
      public async Task Handles_partial_markers_for_openapi()
      {
          var source = """
                       using System;
                       using Vogen;

                       namespace Foo
                       {
                           [ValueObject<int>]
                           public partial struct Vo1;

                           [ValueObject<string>]
                           public partial struct Vo2;
                               
                           [OpenApiMarker<Vo1>]
                           public partial class OpenApiMarkers;

                           [OpenApiMarker<Vo2>]
                           public partial class OpenApiMarkers;
                       }
                       """;

              await new SnapshotRunner<ValueObjectGenerator>()
                  .WithSource(source)
                  .RunOn(TargetFramework.AspNetCore9_0);
      }

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
                     public partial class MyMarkers;

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