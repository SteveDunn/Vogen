
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
                    double doubleValue => new VOTYPE(doubleValue),
                    int intValue => new VOTYPE(intValue),
                    long longValue when longValue < double.MaxValue => new VOTYPE((double)longValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && double.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }