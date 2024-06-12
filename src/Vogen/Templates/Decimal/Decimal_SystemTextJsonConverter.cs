
        /// <summary>
        /// Converts a VOTYPE to or from JSON.
        /// </summary>
        public class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
#if NET5_0_OR_GREATER
__NORMAL__                return VOTYPE.__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize(ref reader, (global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::System.Decimal>)options.GetTypeInfo(typeof(global::System.Decimal))));
#else
__NORMAL__                return VOTYPE.__Deserialize(reader.GetDecimal());
#endif
__STRING__                return VOTYPE.__Deserialize(global::System.Decimal.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__NORMAL__ #if NET5_0_OR_GREATER
__NORMAL__                global::System.Text.Json.JsonSerializer.Serialize(writer, value.Value, options);
__NORMAL__ #else
__NORMAL__                writer.WriteNumberValue(value.Value);
__NORMAL__ #endif
__STRING__                writer.WriteStringValue(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }

#if NET6_0_OR_GREATER
            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return VOTYPE.__Deserialize(global::System.Decimal.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void WriteAsPropertyName(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }
#endif            
        }