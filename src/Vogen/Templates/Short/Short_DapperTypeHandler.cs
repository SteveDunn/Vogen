
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Int16;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    short shortValue => new VOTYPE(shortValue),
                    global::System.Int32 intValue => new VOTYPE((short)intValue),
                    global::System.Int64 longValue => new VOTYPE((short)longValue),

                    string stringValue when  !global::System.String.IsNullOrEmpty(stringValue) && short.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }