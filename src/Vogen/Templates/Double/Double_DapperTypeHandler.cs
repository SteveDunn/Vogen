
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
                    global::System.Double doubleValue => VOTYPE.Deserialize(doubleValue),
                    global::System.Int32 intValue => VOTYPE.Deserialize(intValue),
                    global::System.Int64 longValue when longValue < global::System.Double.MaxValue => VOTYPE.Deserialize((global::System.Double)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Double.TryParse(stringValue, out var result) => VOTYPE.Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }