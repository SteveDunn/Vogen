        
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, VOUNDERLYINGTYPE>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    ut => ut.Value,
                    vo => new VOTYPE(vo),
                    mappingHints
                ) { }
        }