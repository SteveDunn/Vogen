using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Vogen;
using Xunit;

namespace SmallTests;

public class InstanceGenerationTests
{
    public class With_DateTime_and_DateTimeOffset_instances
    {
        public class Which_are_created_with_invalid_ISO8601_strings
        {
            [Theory]
            [InlineData("System.DateTime", "2020:12:13")]
            [InlineData("System.DateTime", "2020-12-13T1:2:3")]
            [InlineData("System.DateTimeOffset", "2020:12:13")]
            [InlineData("System.DateTimeOffset", "2020-12-13T1:2:3")]
            public void It_generates_a_failed_result(string fullName, string input)
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName);

                using var x = new AssertionScope();
                r.Success.Should().BeFalse();
                r.ErrorMessage.Should()
                    .Be(
                        $"Instance value named foo has an attribute with a 'System.String' of '{input}' which cannot be converted to the underlying type of '{fullName}' - String '{input}' was not recognized as a valid DateTime.");
            }
        }

        public class Which_are_created_with_negative_ticks
        {
            [Theory]
            [InlineData("System.DateTime", (int) -1)]
            [InlineData("System.DateTime", -1L)]
            [InlineData("System.DateTime", (int) -2)]
            [InlineData("System.DateTime", -2L)]
            [InlineData("System.DateTimeOffset", (int) -1)]
            [InlineData("System.DateTimeOffset", -2L)]
            public void It_generates_a_failed_result(string fullName, object input)
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName);

                using var x = new AssertionScope();
                r.Success.Should().BeFalse();
                r.ErrorMessage.Should()
                    .Contain(
                        $"Instance value named foo has an attribute with a '{input.GetType().FullName}' of '{input}' which cannot be converted to the underlying type of '{fullName}' - Ticks must be between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks");
            }
        }

        public class Which_are_created_with_valid_ISO8601_strings
        {
            [Fact]
            public void It_generates_a_valid_item_when_given_a_valid_input()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "2020-12-13", typeof(DateTime).FullName);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be(
                        "global::System.DateTime.Parse(\"2020-12-13T00:00:00.0000000\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind)");
                r.ErrorMessage.Should().BeEmpty();
            }
        }

        public class Which_are_created_with_valid_ticks
        {
            [Fact]
            public void It_generates_a_valid_item_when_given_an_int_for_the_ticks()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", (int) 2147483647, typeof(DateTime).FullName);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be("new global::System.DateTime(2147483647,  global::System.DateTimeKind.Utc)");
                r.ErrorMessage.Should().BeEmpty();
            }

            [Fact]
            public void It_generates_a_valid_item_when_given_a_long_for_the_ticks()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", 2147483647L, typeof(DateTime).FullName);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be("new global::System.DateTime(2147483647,  global::System.DateTimeKind.Utc)");
                r.ErrorMessage.Should().BeEmpty();
            }
        }
    }
}