        
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, string>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => System.Text.Json.JsonSerializer.Serialize(vo.Value, default(System.Text.Json.JsonSerializerOptions)),
                    text => new VOTYPE(System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(System.Text.Json.JsonSerializerOptions))),
                    mappingHints
                ) { }
            public EfCoreValueConverter()
                : base(
                    vo => System.Text.Json.JsonSerializer.Serialize(vo.Value, default(System.Text.Json.JsonSerializerOptions)),
                    text => new VOTYPE(System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(System.Text.Json.JsonSerializerOptions))),
                    null
                ) { }
        }