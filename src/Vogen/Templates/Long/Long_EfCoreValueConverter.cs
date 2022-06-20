﻿
        public class EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, global::System.Int64>
        {
            public EfCoreValueConverter() : this(null) { }
            public EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => vo.Value,
                    value => VOTYPE.From(value),
                    mappingHints
                ) { }
        }