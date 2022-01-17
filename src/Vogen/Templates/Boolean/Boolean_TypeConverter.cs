
        class VOTYPETypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(bool) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    bool boolValue => new VOTYPE(boolValue),
                    string stringValue when  !string.IsNullOrEmpty(stringValue) && bool.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(bool) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(bool))
                    {
                        return idValue.Value;
                    }

                    if (destinationType == typeof(string))
                    {
                        return idValue.Value.ToString();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }