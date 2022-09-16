using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Vogen;
using Xunit;

namespace SmallTests.Instances;

public class InstanceGenerationTests
{
    public class With_underlying_Decimal_instance
    {
        [Theory]
        [InlineData((long)0)]
        [InlineData((int)0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_m(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Decimal");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{input}m");
        }
    }

    public class With_underlying_Double_instance
    {
        [Theory]
        [InlineData((long)0)]
        [InlineData(0d)]
        [InlineData((int)0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_m(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Double");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{input}d");
        }
    }

    public class With_underlying_Single_instance
    {
        [Theory]
        [InlineData((long)0)]
        [InlineData(0d)]
        [InlineData((int)0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_f(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Single");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{input}f");
        }
    }

    public class With_underlying_Char_instance
    {
        [Theory]
        [InlineData("1")]
        [InlineData('1')]
        // [InlineData((long)0)]
        // [InlineData(0d)]
        // [InlineData((int)0)]
        public void It_generates_a_successful_result_with_input_surrounded_by_single_quotes(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"'{input}'");
        }

        [Theory]
        [InlineData(0, "\u0000")]
        [InlineData(1, "\u0001")]
        [InlineData(255, "ÿ")]
        [InlineData(256, "Ā")]
        public void It_handles_types_that_can_be_converted_to_char(object input, string expected)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"'{expected}'");
        }

        [Theory]
        [InlineData("-1")]
        [InlineData(1.23)]
        public void It_generates_a_failed_result_when_given_invalid_data(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char");

            using var x = new AssertionScope();
            r.Success.Should().BeFalse();
        }
    }

    public class With_underlying_Byte_instance
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData('1', 49)]
        // [InlineData((long)0)]
        // [InlineData(0d)]
        // [InlineData((int)0)]
        public void It_generates_a_successful_result_with_same_value_as_the_input(object input, byte expected)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{expected}");
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(1, "1")]
        [InlineData(1.23, "1")]
        [InlineData(255, "255")]
        public void It_handles_types_that_can_be_converted(object input, string expected)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{expected}");
        }

        [Theory]
        [InlineData("-1")]
        [InlineData(256)]
        public void It_generates_a_failed_result_when_given_invalid_data(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte");

            using var x = new AssertionScope();
            r.Success.Should().BeFalse();
        }
    }

    public class With_underlying_String_instance
    {
        [Theory]
        [InlineData("a")]
        [InlineData("")]
        public void It_generates_a_successful_result_with_the_input_surrounded_by_quotes(string input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.String");

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"\"{input}\"");
        }
    }

    public class With_underlying_DateTime_and_DateTimeOffset_instances
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
                    .Match(
                        $"Instance value named foo has an attribute with a 'System.String' of '{input}' which cannot be converted to the underlying type of '{fullName}' - * was not recognized as a valid DateTime.");
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

        public class Which_are_created_with_positive_ticks_both_long_and_int
        {
            [Theory]
            [InlineData("System.DateTime", (int) 0)]
            [InlineData("System.DateTime", 0L)]
            [InlineData("System.DateTime", (int) 1_000_000_000)]
            [InlineData("System.DateTime", 1_000_000_000L)]
            [InlineData("System.DateTimeOffset", (int) 0)]
            [InlineData("System.DateTimeOffset", 0L)]
            [InlineData("System.DateTimeOffset", (int) 1_000_000_000)]
            [InlineData("System.DateTimeOffset", 1_000_000_000L)]
            public void It_generates_a_successful_result(string fullName, object input)
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
            }
        }

        public class Which_are_created_with_valid_ISO8601_strings
        {
            [Fact]
            public void It_generates_a_valid_item_when_given_a_valid_DateTime_input()
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

            [Fact]
            public void It_generates_a_valid_item_when_given_a_valid_DateTimeOffset_input()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "2020-12-13", typeof(DateTimeOffset).FullName);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be(
                        "global::System.DateTimeOffset.Parse(\"2020-12-13T00:00:00.0000000+00:00\", null, global::System.Globalization.DateTimeStyles.RoundtripKind)");
                r.ErrorMessage.Should().BeEmpty();
            }
        }
    }
}