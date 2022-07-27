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
                    global::System.Char charValue => VOTYPE.Deserialize(charValue),
                    global::System.Int16 shortValue => VOTYPE.Deserialize((global::System.Char)shortValue),
                    global::System.Int32 intValue => VOTYPE.Deserialize((global::System.Char)intValue),
                    global::System.Int64 longValue => VOTYPE.Deserialize((global::System.Char)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Char.TryParse(stringValue, out var result) => VOTYPE.Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }