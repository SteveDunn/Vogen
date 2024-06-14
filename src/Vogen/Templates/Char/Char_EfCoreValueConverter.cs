
        public class CLASS_PREFIXEfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, global::System.Char>
        {
            public CLASS_PREFIXEfCoreValueConverter() : this(null) { }
            public CLASS_PREFIXEfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => vo.Value,
                    value => VOTYPE.__Deserialize(value),
                    mappingHints
                ) { }
        }