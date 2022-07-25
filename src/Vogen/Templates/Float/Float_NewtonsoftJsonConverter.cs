
        class VOTYPENewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(VOTYPE);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (VOTYPE)value;
                serializer.Serialize(writer, id.Value);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var result = serializer.Deserialize<global::System.float?>(reader);
                return result.HasValue ? VOTYPE.Deserialize(result.Value) : null;
            }
        }