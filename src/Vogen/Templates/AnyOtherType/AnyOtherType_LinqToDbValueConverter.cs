
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, string>
        {
            public LinqToDbValueConverter()
                : base(
                      v => global::System.Text.Json.JsonSerializer.Serialize(v.Value, default(global::System.Text.Json.JsonSerializerOptions)),
                      p => new VOTYPE(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(p, default(global::System.Text.Json.JsonSerializerOptions))),
                      handlesNulls: false)
            { }
        }