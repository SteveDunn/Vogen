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
                    global::System.Char charValue => VOTYPE.From(charValue),
                    global::System.Int16 shortValue => VOTYPE.From((global::System.Char)shortValue),
                    global::System.Int32 intValue => VOTYPE.From((global::System.Char)intValue),
                    global::System.Int64 longValue => VOTYPE.From((global::System.Char)longValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Char.TryParse(stringValue, out var result) => VOTYPE.From(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }