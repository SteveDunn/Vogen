// using System.Threading.Tasks;
// using Shared;
// using Vogen;
//
// namespace SnapshotTests.Markers;
//
// public class OpenApiTests
// {
//      [Fact]
//      public async Task Writes()
//      {
//          var source = """
//                       using System;
//                       using Vogen;
//
//                       namespace Foo
//                       {
//                           [ValueObject<int>]
//                           public partial struct Vo1;
//
//                           [ValueObject<string>]
//                           public partial struct Vo2;
//                               
//                           [OpenApiMarker<Vo1>]
//                           [OpenApiMarker<Vo2>]
//                           public partial class OpenApiMarkers;
//                       }
//                       """;
//
//              await new SnapshotRunner<ValueObjectGenerator>()
//                  .WithSource(source)
//                  .RunOn(TargetFramework.AspNetCore9_0);
//      }
// }