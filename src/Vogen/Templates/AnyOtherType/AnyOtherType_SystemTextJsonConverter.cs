
        class VOTYPESystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
            {
                var primitive = System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(ref reader, options);
                return new VOTYPE(primitive);
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, System.Text.Json.JsonSerializerOptions options)
            {
                System.Text.Json.JsonSerializer.Serialize(writer, value.Value);
            }
        }