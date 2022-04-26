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
                    global::System.Boolean boolValue => new VOTYPE(boolValue),
                    global::System.Int64 longValue when longValue is 0 => new VOTYPE(false),
                    global::System.Int64 longValue when longValue is 1 => new VOTYPE(true),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Boolean.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }