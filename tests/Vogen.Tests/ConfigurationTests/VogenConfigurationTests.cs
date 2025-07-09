using FluentAssertions;
using Xunit;

namespace Vogen.Tests.ConfigurationTests;

public class VogenConfigurationTests
{
    public class Defaulting
    {
        [Fact]
        public void Defaults()
        {
            var instance = VogenConfiguration.DefaultInstance;
            
            instance.Conversions.Should().Be(Conversions.TypeConverter | Conversions.SystemTextJson);
            instance.Customizations.Should().Be(Customizations.None);
            instance.DeserializationStrictness.Should().Be(DeserializationStrictness.Default);
            instance.DebuggerAttributes.Should().Be(DebuggerAttributeGeneration.Full);
            instance.StringComparers.Should().Be(StringComparersGeneration.Omit);
            instance.ToPrimitiveCasting.Should().Be(CastOperator.Explicit);
            instance.FromPrimitiveCasting.Should().Be(CastOperator.Explicit);
            instance.ParsableForPrimitives.Should().Be(ParsableForPrimitives.HoistMethodsAndInterfaces);
            instance.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
            instance.TryFromGeneration.Should().Be(TryFromGeneration.GenerateBoolAndErrorOrMethods);
            instance.IsInitializedMethodGeneration.Should().Be(IsInitializedMethodGeneration.Generate);
            instance.SystemTextJsonConverterFactoryGeneration.Should().Be(SystemTextJsonConverterFactoryGeneration.Generate);
            instance.StaticAbstractsGeneration.Should().Be(StaticAbstractsGeneration.Omit);
            instance.ExplicitlySpecifyTypeInValueObject.Should().Be(false);
            instance.PrimitiveEqualityGeneration.Should().Be(PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
        }
    }
    
    public class DebuggerAttributeGenerationFlag
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Basic),
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Full));

            result.DebuggerAttributes.Should().Be(DebuggerAttributeGeneration.Basic);
        }

        [Fact]
        public void Uses_global_when_local_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Default),
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Basic));

            result.DebuggerAttributes.Should().Be(DebuggerAttributeGeneration.Basic);
        }

        private static VogenConfiguration ConfigWithOmitDebugAs(DebuggerAttributeGeneration debuggerAttributes) =>
            new(
                null,
                null,
                Conversions.Default,
                Customizations.None,
                DeserializationStrictness.Default,
                debuggerAttributes,
                ComparisonGeneration.UseUnderlying,
                StringComparersGeneration.Unspecified,
                CastOperator.Unspecified,
                CastOperator.Unspecified,
                false,
                ParsableForStrings.GenerateMethodsAndInterface,
                ParsableForPrimitives.HoistMethodsAndInterfaces,
                TryFromGeneration.Unspecified,
                IsInitializedMethodGeneration.Unspecified,
                SystemTextJsonConverterFactoryGeneration.Unspecified,
                StaticAbstractsGeneration.Unspecified,
                OpenApiSchemaCustomizations.Unspecified,
                false,
                PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
    }

    public class Primitive_equality_generation
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var local = new ConfigBuilder().WithPrimitiveEqualityGeneration(PrimitiveEqualityGeneration.GenerateMethods).Build();
            var global = new ConfigBuilder().WithPrimitiveEqualityGeneration(PrimitiveEqualityGeneration.GenerateOperatorsAndMethods).Build();
            
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(local, global);

            result.PrimitiveEqualityGeneration.Should().Be(PrimitiveEqualityGeneration.GenerateMethods);
        }

        [Fact]
        public void Uses_global_when_local_not_specified()
        {
            var local = new ConfigBuilder().WithPrimitiveEqualityGeneration(PrimitiveEqualityGeneration.Unspecified).Build();
            var global = new ConfigBuilder().WithPrimitiveEqualityGeneration(PrimitiveEqualityGeneration.GenerateOperatorsAndMethods).Build();
            
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(local, global);

            result.PrimitiveEqualityGeneration.Should().Be(PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
        }
    }

    public class Casting
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var local = ConfigWithCastingAs(toPrimitiveCast: CastOperator.Implicit, fromPrimitiveCast: CastOperator.Explicit);
            var global = ConfigWithCastingAs(toPrimitiveCast: CastOperator.None, fromPrimitiveCast: CastOperator.Implicit);
            
            VogenConfiguration result = CombineConfigurations.CombineAndResolveAnythingUnspecified(localValues: local, globalValues: global);

            result.ToPrimitiveCasting.Should().Be(expected: CastOperator.Implicit);
            result.FromPrimitiveCasting.Should().Be(expected: CastOperator.Explicit);
        }

        [Fact]
        public void Uses_global_when_local_not_specified()
        {
            var local = ConfigWithCastingAs(CastOperator.Unspecified, CastOperator.Unspecified);
            var global = ConfigWithCastingAs(CastOperator.Explicit, CastOperator.Implicit);
            
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(local, global);

            result.ToPrimitiveCasting.Should().Be(CastOperator.Explicit);
            result.FromPrimitiveCasting.Should().Be(CastOperator.Implicit);
        }

        private static VogenConfiguration ConfigWithCastingAs(CastOperator toPrimitiveCast, CastOperator fromPrimitiveCast) =>
            new(
                UnderlyingType: null,
                ValidationExceptionType: null,
                Conversions: Conversions.Default,
                Customizations: Customizations.None,
                DeserializationStrictness: DeserializationStrictness.Default,
                DebuggerAttributes: DebuggerAttributeGeneration.Full,
                Comparison: ComparisonGeneration.UseUnderlying,
                StringComparers: StringComparersGeneration.Unspecified,
                ToPrimitiveCasting: toPrimitiveCast,
                FromPrimitiveCasting: fromPrimitiveCast,
                DisableStackTraceRecordingInDebug: false,
                ParsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
                ParsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
                TryFromGeneration: TryFromGeneration.Unspecified,
                IsInitializedMethodGeneration: IsInitializedMethodGeneration.Unspecified,
                SystemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Unspecified,
                StaticAbstractsGeneration: StaticAbstractsGeneration.Unspecified,
                OpenApiSchemaCustomizations: OpenApiSchemaCustomizations.Unspecified,
                false,
                PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
    }

    public class Conversion
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(ConfigWithOmitConversionsAs(Conversions.EfCoreValueConverter), ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.EfCoreValueConverter);
        }

        [Fact]
        public void Local_beats_global_when_local_is_default_and_global_is_not()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.SystemTextJson | Conversions.TypeConverter),
                ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.SystemTextJson | Conversions.TypeConverter);
        }

        [Fact]
        public void Default_is_correct()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.Default),
                ConfigWithOmitConversionsAs(Conversions.Default));

            result.Conversions.Should().Be(Conversions.SystemTextJson | Conversions.TypeConverter);
        }

        [Fact]
        public void Unspecified_is_Default()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.Unspecified),
                ConfigWithOmitConversionsAs(Conversions.Unspecified));

            result.Conversions.Should().Be(Conversions.Default);
        }

        [Fact]
        public void Unspecified_is_overridable_locally()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.Unspecified),
                ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.NewtonsoftJson);
        }
        [Fact]
        public void Unspecified_is_overridable_globally()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson),
                ConfigWithOmitConversionsAs(Conversions.Unspecified));

            result.Conversions.Should().Be(Conversions.NewtonsoftJson);
        }

        [Fact]
        public void Default_is_combinable_with_other_enum_members()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.Default | Conversions.NewtonsoftJson),
                ConfigWithOmitConversionsAs(Conversions.Unspecified));

            result.Conversions.Should().Be(Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.TypeConverter);
        }

        [Fact]
        public void Unspecified_when_combined_with_other_enum_members_forces_the_value_to_unspecified()
        {
            // This is a bit strange - but because of the -1 value of Unspecified - it works this way
            // And I think that it is ok for this to be the behaviour
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                ConfigWithOmitConversionsAs(Conversions.Unspecified | Conversions.NewtonsoftJson),
                ConfigWithOmitConversionsAs(Conversions.Unspecified | Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.Default);
        }

        private static VogenConfiguration ConfigWithOmitConversionsAs(Conversions conversions) =>
            new(
                null,
                null,
                conversions,
                Customizations.None,
                DeserializationStrictness.Default,
                DebuggerAttributeGeneration.Full,
                ComparisonGeneration.UseUnderlying,
                StringComparersGeneration.Unspecified,
                CastOperator.Unspecified,
                CastOperator.Unspecified,
                false,
                ParsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
                ParsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
                TryFromGeneration.Unspecified,
                IsInitializedMethodGeneration.Unspecified,
                SystemTextJsonConverterFactoryGeneration.Unspecified,
                StaticAbstractsGeneration.Unspecified,
                OpenApiSchemaCustomizations.Unspecified,
                false,
                PrimitiveEqualityGeneration.GenerateOperatorsAndMethods);
    }

    public class Comparable
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                new ConfigBuilder().WithComparable(ComparisonGeneration.Omit).Build(),
                new ConfigBuilder().WithComparable(ComparisonGeneration.UseUnderlying).Build());

            result.Comparison.Should().Be(ComparisonGeneration.Omit);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                new ConfigBuilder().WithComparable(ComparisonGeneration.Default).Build(),
                new ConfigBuilder().WithComparable(ComparisonGeneration.Omit).Build());

            result.Comparison.Should().Be(ComparisonGeneration.Omit);
        }
    }

    public class StringComparersGenerationTests
    {
        [Fact]
        public void Defaults_to_omit()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build());

            result.StringComparers.Should().Be(StringComparersGeneration.Omit);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Generate).Build(), 
                globalValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build());

            result.StringComparers.Should().Be(StringComparersGeneration.Generate);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Generate).Build());

            result.StringComparers.Should().Be(StringComparersGeneration.Generate);
        }
    }

    public class StringParsableGenerationTests
    {
        [Fact]
        public void Defaults_to_generate_methods_and_interface()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.GenerateMethodsAndInterface).Build(), 
                globalValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.GenerateMethodsAndInterface).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }
    }

    public class TryFromGenerationTests
    {
        [Fact]
        public void Defaults_to_generate_both_bool_and_erroror_methods()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.GenerateBoolAndErrorOrMethods);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Omit).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.GenerateBoolMethod).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.Omit);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.GenerateErrorOrMethod).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.GenerateErrorOrMethod);
        }
    }

    public class IsInitializedMethodGenerationTests
    {
        [Fact]
        public void Defaults_to_generate()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Unspecified).Build());

            result.IsInitializedMethodGeneration.Should().Be(IsInitializedMethodGeneration.Generate);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Generate).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Omit).Build());

            result.IsInitializedMethodGeneration.Should().Be(IsInitializedMethodGeneration.Generate);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedMethodGeneration.Omit).Build());

            result.IsInitializedMethodGeneration.Should().Be(IsInitializedMethodGeneration.Omit);
        }
    }

    public class SystemTextJsonConverterFactoryGenerationTests
    {
        [Fact]
        public void Defaults_to_generate()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithSystemTextJsonConverterFactoryGeneration(SystemTextJsonConverterFactoryGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithSystemTextJsonConverterFactoryGeneration(SystemTextJsonConverterFactoryGeneration.Unspecified).Build());

            result.SystemTextJsonConverterFactoryGeneration.Should().Be(SystemTextJsonConverterFactoryGeneration.Generate);
        }
    }

    public class StaticAbstractionGenerationTests
    {
        [Fact]
        public void Defaults_to_omit()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithStaticAbstractsGeneration(StaticAbstractsGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithStaticAbstractsGeneration(StaticAbstractsGeneration.Unspecified).Build());

            result.StaticAbstractsGeneration.Should().Be(StaticAbstractsGeneration.Omit);
        }
    }

    public class SwashbuckleTests
    {
        [Fact]
        public void Defaults_to_omit()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithSwashbuckleSchemaFilterGeneration(OpenApiSchemaCustomizations.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithSwashbuckleSchemaFilterGeneration(OpenApiSchemaCustomizations.Unspecified).Build());

            result.OpenApiSchemaCustomizations.Should().Be(OpenApiSchemaCustomizations.Omit);
        }

        [Fact]
        public void Can_be_overridden_omit()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithSwashbuckleSchemaFilterGeneration(OpenApiSchemaCustomizations.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithSwashbuckleSchemaFilterGeneration(OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter).Build());

            result.OpenApiSchemaCustomizations.Should().Be(OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter);
        }
    }

    public class PrimitiveTypeMustBeExplicitTests
    {
        [Fact]
        public void Defaults_to_false()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithPrimitiveTypeMustBeExplicit(false).Build(), 
                globalValues: new ConfigBuilder().WithPrimitiveTypeMustBeExplicit(true).Build());

            result.ExplicitlySpecifyTypeInValueObject.Should().Be(true);
        }
    }

    public class PrimitiveParsableGenerationTests
    {
        [Fact]
        public void Defaults_to_hoist_methods_and_interfaces_for_primitives()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.HoistMethodsAndInterfaces).Build(), 
                globalValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build());

            result.ParsableForPrimitives.Should().Be(ParsableForPrimitives.HoistMethodsAndInterfaces);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(
                localValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.HoistMethodsAndInterfaces).Build());

            result.ParsableForPrimitives.Should().Be(ParsableForPrimitives.HoistMethodsAndInterfaces);
        }
    }
}