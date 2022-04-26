
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Boolean>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => new VOTYPE(p),
                      handlesNulls: false)
            { }
        }