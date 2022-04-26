        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Byte;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    byte byteValue => new VOTYPE(byteValue),
                    short shortValue => new VOTYPE((byte)shortValue),
                    global::System.Int32 intValue => new VOTYPE((byte)intValue),
                    global::System.Int64 longValue => new VOTYPE((byte)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && byte.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }