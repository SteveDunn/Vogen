
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Int32;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Decimal decimalValue when decimalValue < global::System.Int32.MaxValue && decimalValue % 1 == 0 => VOTYPE.__Deserialize((global::System.Int32) decimalValue)
                    global::System.Int32 intValue => VOTYPE.__Deserialize(intValue),
                    global::System.Int64 longValue when longValue < global::System.Int32.MaxValue => VOTYPE.__Deserialize((global::System.Int32)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Int32.TryParse(stringValue, out var result) => VOTYPE.__Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }