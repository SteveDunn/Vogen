
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Double;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Decimal decimalValue when decimalValue < global::System.Double.MaxValue => VOTYPE.__Deserialize((global::System.Double) decimalValue),
                    global::System.Double doubleValue => VOTYPE.__Deserialize(doubleValue),
                    global::System.Int32 intValue => VOTYPE.__Deserialize(intValue),
                    global::System.Int64 longValue when longValue < global::System.Double.MaxValue => VOTYPE.__Deserialize((global::System.Double)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Double.TryParse(stringValue, out var result) => VOTYPE.__Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }