﻿
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, decimal>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => vo.Value,
                    value => VOTYPE.From(value),
                    mappingHints
                ) { }
            public EfCoreValueConverter()
                : base(
                    vo => vo.Value,
                    value => VOTYPE.From(value),
                    mappingnullHints
                ) { }
        }