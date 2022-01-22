
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.Int16;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    short shortValue => new VOTYPE(shortValue),
                    int intValue => new VOTYPE((short)intValue),
                    long longValue => new VOTYPE((short)longValue),

                    string stringValue when  !string.IsNullOrEmpty(stringValue) && short.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }