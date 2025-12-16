using FluentAssertions;
using Vogen.Generators.Conversions;
using Xunit;

namespace Vogen.Tests;

public class CodeSectionsTests
{
    [Fact]
    public void CanCutSections()
    {
        string s = @"using System.Globalization;

        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            { 
__NORMAL__                return VOTYPE.From(reader.GetDouble());
__STRING__                return VOTYPE.From(global::System.Double.Parse(reader.GetString(), NumberStyles.Any, global::System.CultureInfo.InvariantCulture));
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__NORMAL__                writer.WriteNumberValue(value.Value);
__STRING__                writer.WriteString(value.Value.ToString(global::System.CultureInfo.InvariantCulture));
            }
        }";

        string result = CodeSections.CutSection(s, "__NORMAL__").Replace("\r\n", "\n");

        result.Should().Be(@"using System.Globalization;

        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            { 
__STRING__                return VOTYPE.From(global::System.Double.Parse(reader.GetString(), NumberStyles.Any, global::System.CultureInfo.InvariantCulture));
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__STRING__                writer.WriteString(value.Value.ToString(global::System.CultureInfo.InvariantCulture));
            }
        }".Replace("\r\n", "\n"));
    }

    [Fact]
    public void CanKeepSections()
    {
        string s = @"using System.Globalization;

        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            { 
__STRING__                return VOTYPE.From(global::System.Double.Parse(reader.GetString(), NumberStyles.Any, global::System.CultureInfo.InvariantCulture));
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__STRING__                writer.WriteString(value.Value.ToString(global::System.CultureInfo.InvariantCulture));
            }
        }";

        string result = CodeSections.KeepSection(s, "__STRING__").Replace("\r\n", "\n");

        result.Should().Be(@"using System.Globalization;

        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            { 
                return VOTYPE.From(global::System.Double.Parse(reader.GetString(), NumberStyles.Any, global::System.CultureInfo.InvariantCulture));
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteString(value.Value.ToString(global::System.CultureInfo.InvariantCulture));
            }
        }".Replace("\r\n", "\n"));
    }
}