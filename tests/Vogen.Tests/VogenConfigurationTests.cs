using System;
using FluentAssertions;
using Xunit;

namespace Vogen.Tests
{
    public class VogenConfigurationTests
    {
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
                    StringComparisonGeneration.Unspecified);
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
                    StringComparisonGeneration.Unspecified);
        }

        public class Comparable
        {
            [Fact]
            public void Local_beats_global_when_specified()
            {
                var result = VogenConfiguration.Combine(new ConfigBuilder().WithComparable(ComparisonGeneration.Omit).Build(), new ConfigBuilder().WithComparable(ComparisonGeneration.UseUnderlying).Build());

                result.Comparison.Should().Be(ComparisonGeneration.Omit);
            }

            [Fact]
            public void Global_beats_local_when_local_is_not_specified()
            {
                var result = VogenConfiguration.Combine(new ConfigBuilder().Build(), new ConfigBuilder().WithComparable(ComparisonGeneration.Omit).Build());

                result.Comparison.Should().Be(ComparisonGeneration.Omit);
            }
        }

        public class StringComparisonTests
        {
            [Fact]
            public void Local_beats_global_when_specified()
            {
                var result = VogenConfiguration.Combine(
                    localValues: new ConfigBuilder().WithStringComparison(StringComparisonGeneration.Ordinal).Build(), 
                    globalValues: new ConfigBuilder().WithStringComparison(StringComparisonGeneration.Unspecified).Build());

                result.StringComparison.Should().Be(StringComparisonGeneration.Ordinal);
            }

            [Fact]
            public void Global_beats_local_when_local_is_not_specified()
            {
                var result = VogenConfiguration.Combine(
                    localValues: new ConfigBuilder().WithStringComparison(StringComparisonGeneration.Unspecified).Build(), 
                    globalValues: new ConfigBuilder().WithStringComparison(StringComparisonGeneration.OrdinalIgnoreCase).Build());

                result.StringComparison.Should().Be(StringComparisonGeneration.OrdinalIgnoreCase);
            }
        }

        public class ConfigBuilder
        {
            private VogenConfiguration _c;
            
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
                    _c.StringComparison);
                
                return this;
            }

            public ConfigBuilder WithStringComparison(StringComparisonGeneration g)
            {
                _c = new VogenConfiguration(
                    _c.UnderlyingType,
                    _c.ValidationExceptionType,
                    _c.Conversions,
                    _c.Customizations,
                    _c.DeserializationStrictness,
                    _c.DebuggerAttributes,
                    _c.Comparison,
                    g);
                
                return this;
            }
            
            public VogenConfiguration Build() => _c;
        }
    }
}