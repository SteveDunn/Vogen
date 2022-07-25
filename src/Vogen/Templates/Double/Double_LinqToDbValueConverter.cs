
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Double>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => VOTYPE.Deserialize(p),
                      handlesNulls: false)
            { }
        }