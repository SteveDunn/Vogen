
        public class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Char>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => new VOTYPE(p),
                      handlesNulls: false)
            { }
        }