using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Shared;
using Vogen;

namespace SnapshotTests.OrleansGeneration;

public class OrleansGenerationTests
{
    [Fact]
    public async Task Generates_if_global_attribute_set_and_on_net_8_or_greater()
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

    [Fact]
    public async Task Generates_if_local_attribute_set_and_on_net_8_or_greater()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.None)]

                     namespace OrleansTests;

                     [ValueObject<int>(conversions: Conversions.Orleans)]
                     public partial struct Age;

                     [ValueObject<string>(conversions: Conversions.Orleans)]
                     public partial struct Name;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("Microsoft.Orleans.Serialization", "8.2.0", string.Empty))
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Takes_on_accessibility_of_value_object()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.None)]

                     namespace OrleansTests;

                     [ValueObject<int>(conversions: Conversions.Orleans)]
                     internal partial struct Age;

                     [ValueObject<string>(conversions: Conversions.Orleans)]
                     public partial struct Name;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("Microsoft.Orleans.Serialization", "8.2.0", string.Empty))
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Escapes_wrapper_name()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.None)]

                     namespace @class.@struct.@record;

                     [ValueObject<int>(conversions: Conversions.Orleans)]
                     internal partial struct @class;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("Microsoft.Orleans.Serialization", "8.2.0", string.Empty))
            .IgnoreInitialCompilationErrors()
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Skipped_on_csharp_less_than_12()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(conversions: Conversions.None)]

                     namespace OrleansTests;

                     [ValueObject<int>(conversions: Conversions.Orleans)]
                     public partial struct Age { }

                     [ValueObject<string>(conversions: Conversions.Orleans)]
                     public partial struct Name { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .WithPackage(new NuGetPackage("Microsoft.Orleans.Serialization", "8.2.0", string.Empty))
            .IgnoreInitialCompilationErrors()
            .WithLanguageVersion(LanguageVersion.CSharp11)
            .RunOn(TargetFramework.Net7_0);
    }
}