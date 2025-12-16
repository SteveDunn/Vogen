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
                return DeserializeJson(reader.GetDateTime().ToUniversalTime());
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value.ToUniversalTime());
            }

#if NET6_0_OR_GREATER
            /// <summary>
            /// Converts a VOTYPE to or from JSON.
            /// </summary>
            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return DeserializeJson(global::System.DateTime.ParseExact(reader.GetString(), "O", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind));
            }

            public override void WriteAsPropertyName(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToUniversalTime().ToString("O", global::System.Globalization.CultureInfo.InvariantCulture));
            }
#endif
            DESERIALIZEJSONMETHOD
        }