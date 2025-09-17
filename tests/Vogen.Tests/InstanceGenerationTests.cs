using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

// ReSharper disable RedundantCast

namespace Vogen.Tests;

public static class TestCompilation
{
    public static Compilation Create(
        string source,
        LanguageVersion languageVersion = LanguageVersion.Preview) // or specify a concrete version
    {
        var parseOptions = new CSharpParseOptions(languageVersion);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);

        var references = GetNetCoreReferences();

        return CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static IReadOnlyList<MetadataReference> GetNetCoreReferences()
    {
        // Works on .NET Core/.NET 5+ test runners
        var tpa = (string) AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        var paths = tpa.Split(Path.PathSeparator);

        // Pick a reasonable set of core libs. Adjust if your code needs more.
        HashSet<string> needed =
        [
            "System.Runtime.dll",
            "mscorlib.dll", // sometimes present as a facade
            "netstandard.dll", // for netstandard references
            "System.Private.CoreLib.dll",
            "System.Console.dll",
            "System.Collections.dll",
            "System.Linq.dll",
            "System.Linq.Expressions.dll",
            "System.Text.RegularExpressions.dll",
            "System.Runtime.Extensions.dll",
            "System.Threading.Tasks.dll",
            "System.Memory.dll",
            "System.Private.Uri.dll",
            "System.ComponentModel.Primitives.dll",
            "System.ComponentModel.TypeConverter.dll",
            "System.ObjectModel.dll",
            "System.Runtime.InteropServices.dll",
            "System.Reflection.dll",
            "System.Reflection.Extensions.dll",
            "System.Reflection.Primitives.dll",
            "System.Runtime.Numerics.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            // Add others as needed for your tests (e.g., DateOnly/TimeOnly live in System.Runtime)
        ];

        HashSet<string> p = paths.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var dict = p.ToDictionary(Path.GetFileName, StringComparer.OrdinalIgnoreCase);

        var refs = new List<MetadataReference>();
        foreach (var file in needed)
        {
            if (dict.TryGetValue(file, out var path))
            {
                refs.Add(MetadataReference.CreateFromFile(path));
            }
        }

        // Always include the assembly containing System.Object as a reference
        refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        // If you need specific framework types (e.g., DateOnly/TimeOnly), you can also add their assemblies:
        // refs.Add(MetadataReference.CreateFromFile(typeof(DateOnly).Assembly.Location));

        return refs;
    }

    public static Compilation Instance => Create(
        @"
    using System;
    public class C { public int M() => 42; }
");
}

public class InstanceGenerationTests
{
    private static readonly VogenKnownSymbols _vks = new(TestCompilation.Instance);


    public class With_underlying_Boolean_instance
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void It_generates_a_successful_result(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Boolean", _vks);

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be($"{input!.ToString()!.ToLower()}");
        }
    }

    public class With_underlying_Decimal_instance
    {
        [Theory]
        [InlineData((long) 0)]
        [InlineData((int) 0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_m(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Decimal", _vks);

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be(FormattableString.Invariant($"{input}m"));
        }
    }

    public class With_underlying_Double_instance
    {
        [Theory]
        [InlineData((long) 0)]
        [InlineData(0d)]
        [InlineData((int) 0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_m(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Double", _vks);

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be(FormattableString.Invariant($"{input}d"));
        }
    }

    public class With_underlying_Single_instance
    {
        [Theory]
        [InlineData((long) 0)]
        [InlineData(0d)]
        [InlineData((int) 0)]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData(1)]
        [InlineData(1.23)]
        [InlineData('1')]
        public void It_generates_a_successful_result_with_input_suffixed_with_f(object input)
        {
            InstanceGeneration.BuildResult r =
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Single", _vks);

            using var x = new AssertionScope();
            r.Success.Should().BeTrue();
            r.Value.Should().Be(FormattableString.Invariant($"{input}f"));
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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Char", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.Byte", _vks);

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
                InstanceGeneration.TryBuildInstanceValueAsText("foo", input, "System.String", _vks);

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
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName, _vks);

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
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName, _vks);

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
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", input, fullName, _vks);

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
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "2020-12-13", typeof(DateTime).FullName, _vks);

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
                var timezoneOffset = TimeZoneInfo.Local.BaseUtcOffset;
                var timezoneOffsetExpected
                    = timezoneOffset.Ticks < 0
                        ? $"-{timezoneOffset:hh}:{timezoneOffset:mm}"
                        : $"+{timezoneOffset:hh}:{timezoneOffset:mm}";

                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "2020-12-13", typeof(DateTimeOffset).FullName, _vks);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be(
                        $"global::System.DateTimeOffset.Parse(\"2020-12-13T00:00:00.0000000{timezoneOffsetExpected}\", null, global::System.Globalization.DateTimeStyles.RoundtripKind)");
                r.ErrorMessage.Should().BeEmpty();
            }

            [Fact]
            public void It_generates_a_valid_item_when_given_a_valid_DateOnly_input()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "2029-12-13", typeof(DateOnly).FullName, _vks);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be(
                        $"global::System.DateOnly.ParseExact(\"2029-12-13\", \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture)");
                r.ErrorMessage.Should().BeEmpty();
            }

            [Fact]
            public void It_generates_a_valid_item_when_given_a_valid_TimeOnly_input()
            {
                InstanceGeneration.BuildResult r =
                    InstanceGeneration.TryBuildInstanceValueAsText("foo", "16-31", typeof(TimeOnly).FullName, _vks);

                using var x = new AssertionScope();
                r.Success.Should().BeTrue();
                r.Value.Should()
                    .Be(
                        $"global::System.TimeOnly.ParseExact(\"16-31\", \"HH-mm\", global::System.Globalization.CultureInfo.InvariantCulture)");
                r.ErrorMessage.Should().BeEmpty();
            }
        }
    }
}