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
                var s = reader.GetString();

                return DeserializeJson(s[0]);
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value.ToString());
            }

#if NET6_0_OR_GREATER
            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                var s = reader.GetString();

                return DeserializeJson(s[0]);
            }

            public override void WriteAsPropertyName(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToString());
            }
#endif
            DESERIALIZEJSONMETHOD
        }