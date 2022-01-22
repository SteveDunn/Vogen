
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.Single;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    float floatValue => new VOTYPE(intValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && float.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }