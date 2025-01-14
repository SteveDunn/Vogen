using System.Linq;
using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.StaticAbstracts;

public class StaticAbstractTests
{
    public class Generates_nothing
    {
        [Fact]
        public async Task it_generates_on_net_8_0_with_no_lang_version_specified()
        {
            var source = """
                         using System;
                         using Vogen;

                         [assembly: VogenDefaults(
                            systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                            conversions: Conversions.None, 
                            staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

                         [ValueObject(typeof(Guid))]
                         public partial class MyVo { }
                         """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        }

        [Fact]
        public async Task if_omitted_in_global_config()
        {
            var source = """
                         using System;
                         using Vogen;

                         [assembly: VogenDefaults(
                            systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                            conversions: Conversions.None, 
                            staticAbstractsGeneration: StaticAbstractsGeneration.Omit)]

                         [ValueObject<Guid>]
                         public partial class MyVo { }
                         """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        }

        [Fact]
        public async Task when_using_default_global_attribute()
        {
            var source = """
                         using System;
                         using Vogen;

                         [ValueObject<Guid>]
                         public partial class MyVo { }
                         """;

            await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        }
    }

    [Fact]
    public async Task when_using_implicit_operators()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(
                     systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                     conversions: Conversions.None, 
                     staticAbstractsGeneration: StaticAbstractsGeneration.FactoryMethods | StaticAbstractsGeneration.EqualsOperators | StaticAbstractsGeneration.ImplicitCastFromPrimitive | StaticAbstractsGeneration.ImplicitCastToPrimitive)]

                     [ValueObject<Guid>(toPrimitiveCasting: CastOperator.Implicit, fromPrimitiveCasting: CastOperator.Implicit)]
                     public partial class MyVo { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Omits_equality_operators_to_primitives_if_specified_in_global_config()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(
                        primitiveEqualityGeneration: PrimitiveEqualityGeneration.Omit, 
                        conversions: Conversions.None, 
                        toPrimitiveCasting: CastOperator.Implicit,
                        fromPrimitiveCasting: CastOperator.Implicit,
                        staticAbstractsGeneration: StaticAbstractsGeneration.FactoryMethods | StaticAbstractsGeneration.EqualsOperators | StaticAbstractsGeneration.ImplicitCastFromPrimitive | StaticAbstractsGeneration.ImplicitCastToPrimitive)]

                     [ValueObject<Guid>]
                     public partial class MyVo { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task Emits_equality_operators_to_primitives_if_specified_in_global_config()
    {
        var source = """
                     using System;
                     using Vogen;

                     [assembly: VogenDefaults(
                        primitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods, 
                        conversions: Conversions.None,
                        toPrimitiveCasting: CastOperator.Implicit,
                        fromPrimitiveCasting: CastOperator.Implicit,
                        staticAbstractsGeneration: StaticAbstractsGeneration.FactoryMethods | 
                            StaticAbstractsGeneration.EqualsOperators | 
                            StaticAbstractsGeneration.ImplicitCastFromPrimitive | 
                            StaticAbstractsGeneration.ImplicitCastToPrimitive)]

                     [ValueObject<Guid>]
                     public partial class MyVo { }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }


    [Theory]
    [InlineData("struct")]
    [InlineData("class")]
    [InlineData("record struct")]
    [InlineData("record class")]
    public async Task Generates_code_for_different_types(string type)
    {
        var source = $$"""
                       using System;
                       using Vogen;

                       [assembly: VogenDefaults(
                       systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                       conversions: Conversions.None, 
                       staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

                       [ValueObject<Guid>]
                       public partial {{type}} MyVo { }

                       """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(type)))
            .RunOn(TargetFramework.Net8_0);
    }

    [Theory]
    [InlineData("ValueObjectsDeriveFromTheInterface", "EqualsOperators")]
    [InlineData("ValueObjectsDeriveFromTheInterface", "FactoryMethods")]
    [InlineData("ValueObjectsDeriveFromTheInterface", "EqualsOperators", "FactoryMethods")]
    [InlineData("ValueObjectsDeriveFromTheInterface", "InstanceMethodsAndProperties")]
    [InlineData("ValueObjectsDeriveFromTheInterface", "InstanceMethodsAndProperties", "EqualsOperators", "FactoryMethods")]
    [InlineData("ValueObjectsDeriveFromTheInterface", "InstanceMethodsAndProperties", "FactoryMethods", "EqualsOperators")]
    public async Task Generates_code_for_different_variations(params string[] flags)
    {
        string attrs = string.Join(" | ", flags.Select(f => $"StaticAbstractsGeneration.{f}"));
        var source = $$"""
                       using System;
                       using Vogen;

                       [assembly: VogenDefaults(
                       systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
                       conversions: Conversions.None, 
                       staticAbstractsGeneration: {{attrs}})]

                       [ValueObject<Guid>]
                       public partial struct MyVo { }

                       """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseFileName(TestHelper.ShortenForFilename(attrs)))
            .RunOn(TargetFramework.Net8_0);
    }
}