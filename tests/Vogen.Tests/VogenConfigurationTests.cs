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
                    debuggerAttributes);
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
                    DebuggerAttributeGeneration.Full);
        }
    }
}