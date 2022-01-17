
        class VOTYPESystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
            {
                return new VOTYPE(reader.GetBoolean());
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteBooleanValue(value.Value);
            }
        }