
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    long longValue => new VOTYPE(longValue),
                    int intValue => new VOTYPE(intValue),
                    short shortValue => new VOTYPE(shortValue),
                    string stringValue when  !string.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }