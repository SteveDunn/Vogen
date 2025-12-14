using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/869
public class Bug869Tests
{
    [Theory]
    [InlineData("partial class")]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task Colliding(string type)
    {
        var source = $$"""
                     namespace NuGet
                     {
                         public class Type1;
                     }
                     
                     namespace ConsumerTests
                     {
                         using Type1 = global::NuGet.Type1;
                         using Vogen;
                     
                         public class NuGet;
                     
                         [ValueObject<Type1>]
                         internal {{type}} MyVo
                         {
                             public static MyVo DefaultInstance => From(new Type1());
                         }
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(type)).IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task Colliding2()
    {
        var source = $$"""
                     namespace NuGet
                     {
                         public class Type1;
                     }
                     
                     namespace ConsumerTests
                     {
                         using Type1 = global::NuGet.Type1;
                         using Vogen;
                     
                         public class NuGet;
                     
                         [ValueObject<Type1>]
                         internal partial class MyVo
                         {
                             public static MyVo DefaultInstance => From(new Type1());
                         }
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .IgnoreInitialCompilationErrors()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
}