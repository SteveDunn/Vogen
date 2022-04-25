        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.Byte;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    byte byteValue => new VOTYPE(byteValue),
                    short shortValue => new VOTYPE((byte)shortValue),
                    int intValue => new VOTYPE((byte)intValue),
                    long longValue => new VOTYPE((byte)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && byte.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }