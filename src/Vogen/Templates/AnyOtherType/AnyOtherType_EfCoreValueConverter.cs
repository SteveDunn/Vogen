        
        public class EfCoreValueConverter : global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, global::System.String>
        {
            public EfCoreValueConverter() : this(null) { }
            public EfCoreValueConverter(global::Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => global::System.Text.Json.JsonSerializer.Serialize(vo.Value, default(global::System.Text.Json.JsonSerializerOptions)),
                    text => VOTYPE.__Deserialize(global::System.Text.Json.JsonSerializer.Deserialize<VOUNDERLYINGTYPE>(text, default(global::System.Text.Json.JsonSerializerOptions))),
                    mappingHints
                ) { }
        }