using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.EfCoreGeneration;

public class EfCoreGenerationTests
{
    [Fact]
    public async Task Writes_efcore_converters_if_attribute_present_and_on_net_8_or_greater()
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
                     }

                     namespace Bar
                     {
                         [ValueObject<int>]
                         public partial struct Vo1;

                         [ValueObject<string>]
                         public partial struct Vo2;
                     }
                     
                     namespace Baz1
                     {
                         [EfCoreConverter<Foo.Vo1>]
                         [EfCoreConverter<Foo.Vo2>]
                         [EfCoreConverter<Bar.Vo1>]
                         [EfCoreConverter<Bar.Vo2>]
                         public partial class EfCoreConverters;
                     }

                     namespace Baz2
                     {
                         [EfCoreConverter<Foo.Vo1>]
                         [EfCoreConverter<Foo.Vo2>]
                         [EfCoreConverter<Bar.Vo1>]
                         [EfCoreConverter<Bar.Vo2>]
                         public partial class EfCoreConverters;
                     }
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Writes_efcore_converters_for_escaped_types()
    {
        var source = """
                     using System;
                     using Vogen;
                     
                     namespace @int
                     {
                         [ValueObject<byte>]
                         public partial struct @byte;

                         [ValueObject<string>]
                         public partial record struct @string;
                     }

                     namespace Whatever
                     {
                         [ValueObject<int>]
                         public partial struct MyVo1;

                         [ValueObject<string>]
                         public partial class MyVo2;

                         [ValueObject<string>]
                         public readonly partial struct MyVo3;

                         [ValueObject<int>]
                         public readonly partial struct MyVo4;

                         [ValueObject<int>]
                         public readonly partial record struct MyVo5;

                         [ValueObject<int>]
                         public partial record class MyVo6;

                         [ValueObject<int>]
                         public partial record struct @int;

                         [ValueObject<int>]
                         public partial record struct @byte;

                         [ValueObject(typeof(int))]
                         public partial record MyInt;
                     
                         [EfCoreConverter<MyVo1>]
                         [EfCoreConverter<MyVo2>]
                         [EfCoreConverter<MyVo3>]
                         [EfCoreConverter<MyVo4>]
                         [EfCoreConverter<MyVo5>]
                         [EfCoreConverter<MyVo6>]
                         [EfCoreConverter<@int>]
                         [EfCoreConverter<@byte>]
                         [EfCoreConverter<MyInt>]
                         public partial class EfCoreConverters;
                     }
                     
                     namespace @byte
                     {
                         [EfCoreConverter<@int.@byte>]
                         [EfCoreConverter<@int.@string>]
                         public partial class EfCoreConverters;
                     }
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Writes_efcore_converters_that_respect_namespaces()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace Namespace1
                     {
                         [ValueObject<int>(conversions: Conversions.EfCoreValueConverter)]
                         public partial class MyVo1 { }
                     
                         [EfCoreConverter<MyVo1>]
                         public partial class EfCoreConverters { }
                     }

                     namespace Namespace2
                     {
                         [ValueObject<int>(conversions: Conversions.EfCoreValueConverter)]
                         public partial class MyVo1 { }
                     
                         [EfCoreConverter<MyVo1>]
                         public partial class EfCoreConverters { }
                     }
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Uses_same_accessibility_as_the_placeholder_class()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace Namespace1
                     {
                         [ValueObject<int>(conversions: Conversions.EfCoreValueConverter)]
                         public partial class MyVo1 { }
                     
                         [EfCoreConverter<MyVo1>]
                         public partial class EfCoreConverters { }
                     }

                     namespace Namespace2
                     {
                         [ValueObject<int>(conversions: Conversions.EfCoreValueConverter)]
                         public partial class MyVo1 { }
                     
                         [EfCoreConverter<MyVo1>]
                         internal partial class EfCoreConverters { }
                     }
                     """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
    }
}