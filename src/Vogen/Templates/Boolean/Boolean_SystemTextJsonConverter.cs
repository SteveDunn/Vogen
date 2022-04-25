﻿
        class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return new VOTYPE(reader.GetBoolean());
            }

            public override void Write(global::System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteBooleanValue(value.Value);
            }
        }