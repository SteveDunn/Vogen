
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Single;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Single floatValue => new VOTYPE(floatValue),
                    double doubleValue when doubleValue < float.MaxValue => new VOTYPE((float)doubleValue),
                    global::System.Int32 intValue => new VOTYPE(intValue),
                    global::System.Int64 longValue when longValue < float.MaxValue => new VOTYPE((float)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && float.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }