
        /// <summary>
        /// Converts a VOTYPE to or from JSON.
        /// </summary>
        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                var primitive = global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(ref reader, options);
                return VOTYPE.__Deserialize(primitive);
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                global::System.Text.Json.JsonSerializer.Serialize(writer, value.Value);
            }

#if NET6_0_OR_GREATER
            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                var primitive = global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(ref reader, options);
                return VOTYPE.__Deserialize(primitive);
            }

            public override void WriteAsPropertyName(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(global::System.Text.Json.JsonSerializer.Serialize(value.Value));
            }
#endif            
        }