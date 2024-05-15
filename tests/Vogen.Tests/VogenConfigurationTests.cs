using FluentAssertions;
using Xunit;

namespace Vogen.Tests;

public class VogenConfigurationTests
{
    public class Defaulting
    {
        [Fact]
        public void Defaults()
        {
            var instance = VogenConfiguration.DefaultInstance;
            
            instance.Conversions.Should().Be(Conversions.Default);
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
                StaticAbstractsGeneration.Unspecified);
    }

    public class Casting
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var local = ConfigWithCastingAs(CastOperator.Implicit, CastOperator.Explicit);
            var global = ConfigWithCastingAs(CastOperator.None, CastOperator.Implicit);
            
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(local, global);

            result.ToPrimitiveCasting.Should().Be(CastOperator.Implicit);
            result.FromPrimitiveCasting.Should().Be(CastOperator.Explicit);
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
                underlyingType: null,
                validationExceptionType: null,
                conversions: Conversions.Default,
                customizations: Customizations.None,
                deserializationStrictness: DeserializationStrictness.Default,
                debuggerAttributes: DebuggerAttributeGeneration.Full,
                comparison: ComparisonGeneration.UseUnderlying,
                stringComparers: StringComparersGeneration.Unspecified,
                toPrimitiveCasting: toPrimitiveCast,
                fromPrimitiveCasting: fromPrimitiveCast,
                disableStackTraceRecordingInDebug: false,
                parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
                parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
                tryFromGeneration: TryFromGeneration.Unspecified,
                isInitializedMethodGeneration: IsInitializedMethodGeneration.Unspecified,
                systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Unspecified,
                staticAbstractsGeneration: StaticAbstractsGeneration.Unspecified);
    }

    public class Conversion
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = CombineConfigurations.CombineAndResolveAnythingUnspecified(ConfigWithOmitConversionsAs(Conversions.EfCoreValueConverter), ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.EfCoreValueConverter);
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
                parsableForStrings: ParsableForStrings.GenerateMethodsAndInterface,
                parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
                TryFromGeneration.Unspecified,
                IsInitializedMethodGeneration.Unspecified,
                SystemTextJsonConverterFactoryGeneration.Unspecified,
                StaticAbstractsGeneration.Unspecified);
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

    public class ConfigBuilder
    {
        private VogenConfiguration _c = VogenConfiguration.DefaultInstance;

        public ConfigBuilder WithComparable(ComparisonGeneration comparable)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                comparable,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithStringComparersGeneration(StringComparersGeneration g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                g,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithTryFromGeneration(TryFromGeneration g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                g,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithIsInitializedGeneration(IsInitializedMethodGeneration g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                g,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithSystemTextJsonConverterFactoryGeneration(SystemTextJsonConverterFactoryGeneration g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                g,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithParsableStringGeneration(ParsableForStrings g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                g,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithParsablePrimitiveGeneration(ParsableForPrimitives g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                g,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                _c.StaticAbstractsGeneration);
                
            return this;
        }

        public ConfigBuilder WithStaticAbstractsGeneration(StaticAbstractsGeneration g)
        {
            _c = new VogenConfiguration(
                _c.UnderlyingType,
                _c.ValidationExceptionType,
                _c.Conversions,
                _c.Customizations,
                _c.DeserializationStrictness,
                _c.DebuggerAttributes,
                _c.Comparison,
                _c.StringComparers,
                _c.ToPrimitiveCasting,
                _c.FromPrimitiveCasting,
                _c.DisableStackTraceRecordingInDebug,
                _c.ParsableForStrings,
                _c.ParsableForPrimitives,
                _c.TryFromGeneration,
                _c.IsInitializedMethodGeneration,
                _c.SystemTextJsonConverterFactoryGeneration,
                g);
                
            return this;
        }
            
        public VogenConfiguration Build() => _c;
    }
}