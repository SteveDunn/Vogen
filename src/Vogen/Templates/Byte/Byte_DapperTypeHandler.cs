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
                    global::System.Byte byteValue => new VOTYPE(byteValue),
                    global::System.Int16 shortValue => new VOTYPE((global::System.Byte)shortValue),
                    global::System.Int32 intValue => new VOTYPE((global::System.Byte)intValue),
                    global::System.Int64 longValue => new VOTYPE((global::System.Byte)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Byte.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }