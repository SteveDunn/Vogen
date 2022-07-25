
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Byte>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => VOTYPE.Deserialize(p),
                      handlesNulls: false)
            { }
        }