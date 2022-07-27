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
                    global::System.Byte byteValue => VOTYPE.Deserialize(byteValue),
                    global::System.Int16 shortValue => VOTYPE.Deserialize((global::System.Byte)shortValue),
                    global::System.Int32 intValue => VOTYPE.Deserialize((global::System.Byte)intValue),
                    global::System.Int64 longValue => VOTYPE.Deserialize((global::System.Byte)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Byte.TryParse(stringValue, out var result) => VOTYPE.Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }