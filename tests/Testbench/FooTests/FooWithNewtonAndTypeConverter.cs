namespace Testbench.FooTests;

[System.Text.Json.Serialization.JsonConverter(typeof(FooSystemTextJsonConverter))]
[Newtonsoft.Json.JsonConverter(typeof(FooNewtonsoftJsonConverter))]
public struct FooWithNewtonAndTypeConverter
{
    public Bar Value { get; set; }

    class FooNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(FooWithNewtonAndTypeConverter);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (FooWithNewtonAndTypeConverter?) value;
            serializer.Serialize(writer, id?.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return FooWithNewtonAndTypeConverter.From(serializer.Deserialize<Bar>(reader));
        }
    }

    class FooSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<FooWithNewtonAndTypeConverter>
    {
        public override FooWithNewtonAndTypeConverter Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            var primitive = System.Text.Json.JsonSerializer.Deserialize<Bar>(ref reader, options);
            return FooWithNewtonAndTypeConverter.From(primitive);
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, FooWithNewtonAndTypeConverter value, System.Text.Json.JsonSerializerOptions options)
        {
            System.Text.Json.JsonSerializer.Serialize(writer, value.Value);
        }
    }

    public static FooWithNewtonAndTypeConverter From(Bar bar)
    {
        return new FooWithNewtonAndTypeConverter {Value = bar};
    }
}