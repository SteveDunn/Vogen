
        public partial class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.String>
        {
            public LinqToDbValueConverter()
                : base(
                      v => global::System.Text.Json.JsonSerializer.Serialize(v.Value, default(global::System.Text.Json.JsonSerializerOptions)),
                      p => VOTYPE.__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(p, default(global::System.Text.Json.JsonSerializerOptions))),
                      handlesNulls: false)
            { }
        }