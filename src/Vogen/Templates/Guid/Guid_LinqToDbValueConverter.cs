
        public partial class LinqToDbValueConverter : global::LinqToDB.Common.ValueConverter<VOTYPE, global::System.Guid>
        {
            public LinqToDbValueConverter()
                : base(
                      v => v.Value,
                      p => VOTYPE.__Deserialize(p),
                      handlesNulls: false)
            { }
        }