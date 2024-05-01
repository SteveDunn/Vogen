#if NET7_0_OR_GREATER_DO_NOT_USE
        class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return VOTYPE.__Deserialize(reader.GetDateOnly());
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value);
            }
        }
#elif NET6_0_OR_GREATER
        class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return new VOTYPE(global::System.DateOnly.ParseExact(reader.GetString(), "yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return new VOTYPE(global::System.DateOnly.ParseExact(reader.GetString(), "yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void WriteAsPropertyName(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToString("yyyy-MM-dd", global::System.Globalization.CultureInfo.InvariantCulture));
            }
        }
#endif

