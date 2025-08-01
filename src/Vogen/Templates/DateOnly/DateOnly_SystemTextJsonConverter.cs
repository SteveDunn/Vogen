#if NET6_0_OR_GREATER
        /// <summary>
        /// Converts a VOTYPE to or from JSON.
        /// </summary>
        public partial class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
__HANDLE_NULL__ #if NET5_0_OR_GREATER
__HANDLE_NULL__            public override bool HandleNull => true;
__HANDLE_NULL__ #endif
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return DeserializeJson(global::System.DateOnly.ParseExact(reader.GetString(), "yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return DeserializeJson(global::System.DateOnly.ParseExact(reader.GetString(), "yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void WriteAsPropertyName(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToString("yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            DESERIALIZEJSONMETHOD
        }
#endif

