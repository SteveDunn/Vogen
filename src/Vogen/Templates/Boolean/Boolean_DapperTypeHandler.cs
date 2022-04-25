        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.Boolean;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    bool boolValue => new VOTYPE(boolValue),
                    long longValue when longValue is 0 => new VOTYPE(false),
                    long longValue when longValue is 1 => new VOTYPE(true),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && bool.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }