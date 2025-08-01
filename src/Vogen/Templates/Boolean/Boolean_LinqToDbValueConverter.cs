
        public partial class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Boolean>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => VOTYPE.__Deserialize(p),
                      handlesNulls: false)
            { }
        }