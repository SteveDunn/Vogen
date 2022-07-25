
        class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
__NORMAL__                return VOTYPE.Deserialize(reader.GetInt64());
__STRING__                return VOTYPE.Deserialize(global::System.Int64.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__NORMAL__                writer.WriteNumberValue(value.Value);
__STRING__                writer.WriteStringValue(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }
        }