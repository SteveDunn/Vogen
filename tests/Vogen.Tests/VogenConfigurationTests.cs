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
        }
    }
    

    public class DebuggerAttributeGenerationFlag
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Basic),
                ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Full));

            result.DebuggerAttributes.Should().Be(DebuggerAttributeGeneration.Basic);
        }

        [Fact]
        public void Uses_global_when_local_not_specified()
        {
            var result = VogenConfiguration.Combine(ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Default), ConfigWithOmitDebugAs(DebuggerAttributeGeneration.Basic));

            result.DebuggerAttributes.Should().Be(DebuggerAttributeGeneration.Basic);
        }

        private static VogenConfiguration ConfigWithOmitDebugAs(DebuggerAttributeGeneration debuggerAttributes) =>
            new VogenConfiguration(
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
                IsInitializedGeneration.Unspecified);
    }

    public class Casting
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var local = ConfigWithCastingAs(CastOperator.Implicit, CastOperator.Explicit);
            var global = ConfigWithCastingAs(CastOperator.None, CastOperator.Implicit);
            
            var result = VogenConfiguration.Combine(local, global);

            result.ToPrimitiveCasting.Should().Be(CastOperator.Implicit);
            result.FromPrimitiveCasting.Should().Be(CastOperator.Explicit);
        }

        [Fact]
        public void Uses_global_when_local_not_specified()
        {
            var local = ConfigWithCastingAs(CastOperator.Unspecified, CastOperator.Unspecified);
            var global = ConfigWithCastingAs(CastOperator.Explicit, CastOperator.Implicit);
            
            var result = VogenConfiguration.Combine(local, global);

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
                isInitializedGeneration: IsInitializedGeneration.Unspecified);
    }

    public class Conversion
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(ConfigWithOmitConversionsAs(Conversions.EfCoreValueConverter), ConfigWithOmitConversionsAs(Conversions.NewtonsoftJson));

            result.Conversions.Should().Be(Conversions.EfCoreValueConverter);
        }

        private static VogenConfiguration ConfigWithOmitConversionsAs(Conversions conversions) =>
            new VogenConfiguration(
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
                IsInitializedGeneration.Unspecified);
    }

    public class Comparable
    {
        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                new ConfigBuilder().WithComparable(ComparisonGeneration.Omit).Build(),
                new ConfigBuilder().WithComparable(ComparisonGeneration.UseUnderlying).Build());

            result.Comparison.Should().Be(ComparisonGeneration.Omit);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
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
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build());

            result.StringComparers.Should().Be(StringComparersGeneration.Omit);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Generate).Build(), 
                globalValues: new ConfigBuilder().WithStringComparersGeneration(StringComparersGeneration.Unspecified).Build());

            result.StringComparers.Should().Be(StringComparersGeneration.Generate);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
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
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.GenerateMethodsAndInterface).Build(), 
                globalValues: new ConfigBuilder().WithParsableStringGeneration(ParsableForStrings.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
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
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.GenerateBoolAndErrorOrMethods);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Omit).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.GenerateBoolMethod).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.Omit);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithTryFromGeneration(TryFromGeneration.GenerateErrorOrMethod).Build());

            result.TryFromGeneration.Should().Be(TryFromGeneration.GenerateErrorOrMethod);
        }
    }

    public class IsInitializedGenerationTests
    {
        [Fact]
        public void Defaults_to_generate()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Unspecified).Build());

            result.IsInitializedGeneration.Should().Be(IsInitializedGeneration.Generate);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Generate).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Omit).Build());

            result.IsInitializedGeneration.Should().Be(IsInitializedGeneration.Generate);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithIsInitializedGeneration(IsInitializedGeneration.Omit).Build());

            result.IsInitializedGeneration.Should().Be(IsInitializedGeneration.Omit);
        }
    }

    public class PrimitiveParsableGenerationTests
    {
        [Fact]
        public void Defaults_to_hoist_methods_and_interfaces_for_primitives()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build(), 
                globalValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build());

            result.ParsableForStrings.Should().Be(ParsableForStrings.GenerateMethodsAndInterface);
        }

        [Fact]
        public void Local_beats_global_when_specified()
        {
            var result = VogenConfiguration.Combine(
                localValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.HoistMethodsAndInterfaces).Build(), 
                globalValues: new ConfigBuilder().WithParsablePrimitiveGeneration(ParsableForPrimitives.Unspecified).Build());

            result.ParsableForPrimitives.Should().Be(ParsableForPrimitives.HoistMethodsAndInterfaces);
        }

        [Fact]
        public void Global_beats_local_when_local_is_not_specified()
        {
            var result = VogenConfiguration.Combine(
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
                _c.IsInitializedGeneration);
                
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
                _c.IsInitializedGeneration);
                
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
                _c.IsInitializedGeneration);
                
            return this;
        }

        public ConfigBuilder WithIsInitializedGeneration(IsInitializedGeneration g)
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
                g);
                
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
                _c.IsInitializedGeneration);
                
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
                _c.IsInitializedGeneration);
                
            return this;
        }
            
        public VogenConfiguration Build() => _c;
    }
}