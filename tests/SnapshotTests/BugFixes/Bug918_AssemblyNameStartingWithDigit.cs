using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/918
public class Bug918_AssemblyNameStartingWithDigit
{
    private const string Source = """
                                  using System;
                                  using Vogen;
                                  using System.Text.Json;

                                  namespace MyNamespace;

                                  [ValueObject(typeof(int), conversions: Conversions.SystemTextJson)]
                                  public partial struct MyVo;
                                  """;

    [Fact]
    public async Task Assembly_name_starting_with_digit_is_sanitised()
    {
        await new SnapshotRunner<ValueObjectGenerator>()
            .WithAssemblyName("1MyProject")
            .WithSource(Source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task RootNamespace_overrides_assembly_name()
    {
        await new SnapshotRunner<ValueObjectGenerator>()
            .WithAssemblyName("1MyProject")
            .WithBuildProperty("build_property.RootNamespace", "OneProject")
            .WithSource(Source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}
