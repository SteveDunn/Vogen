
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, string>
        {
            public LinqToDbValueConverter()
                : base(
                      v => System.Text.Json.JsonSerializer.Serialize(v.Value, default(System.Text.Json.JsonSerializerOptions)),
                      p => new VOTYPE(System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(p, default(System.Text.Json.JsonSerializerOptions))),
                      handlesNulls: false)
            { }
        }