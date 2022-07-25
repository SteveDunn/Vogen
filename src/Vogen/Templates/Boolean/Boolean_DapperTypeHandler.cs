        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Boolean;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Boolean boolValue => VOTYPE.From(boolValue),
                    global::System.Int64 longValue when longValue is 0 => VOTYPE.From(false),
                    global::System.Int64 longValue when longValue is 1 => VOTYPE.From(true),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Boolean.TryParse(stringValue, out var result) => VOTYPE.From(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }