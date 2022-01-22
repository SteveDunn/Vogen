
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = System.Data.DbType.String;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    string stringValue => new VOTYPE(stringValue),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }