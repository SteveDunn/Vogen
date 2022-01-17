﻿
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, string>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    id => id.Value,
                    value => new VOTYPE(value),
                    mappingHints
                ) { }
        }