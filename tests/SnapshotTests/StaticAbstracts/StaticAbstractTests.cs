using System.Threading.Tasks;
using Shared;
using VerifyXunit;
using Vogen;

namespace SnapshotTests.StaticAbstracts;

[UsesVerify]
public class StaticAbstractTests
{
    [Fact]
    public async Task Generates_interface_definition_for_a_class_on_c_sharp_12()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject<Guid>]
                     public partial class MyVo { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .UseGeneratedInterfacesFile()
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Generates_interface_definition_for_a_struct_on_c_sharp_12()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject<Guid>]
                     public partial struct MyVo { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .UseGeneratedInterfacesFile()
            .RunOn(TargetFramework.Net8_0);
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("class")]
    [InlineData("record struct")]
    [InlineData("record class")]
    public async Task Generates_interface_definition_on_c_sharp_12(string type)
    {
        var source = $$"""

                       using System;
                       using Vogen;

                       [assembly: VogenDefaults(customizations: Customizations.AddFactoryMethodForGuids)]
                       
                         [ValueObject<Guid>]
                         public partial {{type}} MyVo { }

                       """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(type)))
            .RunOn(TargetFramework.Net8_0);
    }
}
