        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.StringFixedLength;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    char charValue => new VOTYPE(charValue),
                    short shortValue => new VOTYPE((char)shortValue),
                    global::System.Int32 intValue => new VOTYPE((char)intValue),
                    global::System.Int64 longValue => new VOTYPE((char)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && char.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }