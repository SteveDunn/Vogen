        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.StringFixedLength;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    char charValue => new VOTYPE(charValue),
                    short shortValue => new VOTYPE((char)shortValue),
                    int intValue => new VOTYPE((char)intValue),
                    long longValue => new VOTYPE((char)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && char.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }