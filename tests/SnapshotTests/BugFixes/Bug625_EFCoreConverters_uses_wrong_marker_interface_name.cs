using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/625
public class Bug625_EFCoreConverters_uses_wrong_marker_interface_name
{
    [Fact]
    public async Task A_marker_interface_not_named_EfCoreConverters_is_still_referenced_and_that_name_is_used_in_the_generated_code()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     [assembly: VogenDefaults(
                     systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                     conversions: Conversions.SystemTextJson | 
                                  Conversions.TypeConverter | 
                                  Conversions.SystemTextJson)]

                     namespace Foo;
                     
                     [ValueObject<DateTime>]
                     public partial struct HireDate;

                     [ValueObject<string>]
                     public partial struct Name;

                     [ValueObject<int>]
                     public partial struct Age;
                     
                     [EfCoreConverter<HireDate>]
                     [EfCoreConverter<Name>]
                     [EfCoreConverter<Age>]
                     internal sealed partial class VogenEfCoreConverters;
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOn(TargetFramework.Net8_0);
    }
}