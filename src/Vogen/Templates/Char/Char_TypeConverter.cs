
        class VOTYPETypeConverter : global::System.ComponentModel.TypeConverter
        {
            public override global::System.Boolean CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(global::System.Char) || sourceType == typeof(global::System.String) || base.CanConvertFrom(context, sourceType);
            }

            public override global::System.Object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value)
            {
                return value switch
                {
                    global::System.Char charValue => new VOTYPE(charValue),
                    global::System.Int16 shortValue => new VOTYPE((global::System.Char)shortValue),
                    global::System.Int32 intValue => new VOTYPE((global::System.Char)intValue),
                    global::System.Int64 longValue => new VOTYPE((global::System.Char)longValue),
                    global::System.String stringValue when  !global::System.String.IsNullOrEmpty(stringValue) && global::System.Char.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(global::System.Char) || sourceType == typeof(global::System.String) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value, global::System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(global::System.Char))
                    {
                        return idValue.Value;
                    }

                    if (destinationType == typeof(global::System.String))
                    {
                        return idValue.Value.ToString();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }