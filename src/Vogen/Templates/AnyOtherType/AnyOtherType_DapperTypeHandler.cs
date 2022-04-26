
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.Value = global::System.Text.Json.JsonSerializer.Serialize(value.Value);
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.String stringValue =>
                        new VOTYPE(global::System.Text.Json.JsonSerializer.Deserialize<Bar>(stringValue)),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }