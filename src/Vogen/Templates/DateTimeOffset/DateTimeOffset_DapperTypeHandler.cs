
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.DateTimeOffset;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.DateTimeOffset dtValue => VOTYPE.__Deserialize(dtValue),
                    global::System.String stringValue when 
                        !global::System.String.IsNullOrEmpty(stringValue) &&
                        global::System.DateTimeOffset.TryParseExact(stringValue, "yyyy-MM-dd HH:mm:ss.fffffff", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None, out var result) => VOTYPE.__Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }