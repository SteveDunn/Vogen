
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Guid;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Guid guidValue => VOTYPE.Deserialize(guidValue),
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.Guid.TryParse(stringValue, out var result) => VOTYPE.Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }