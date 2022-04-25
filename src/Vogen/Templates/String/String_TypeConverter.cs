
        class VOTYPETypeConverter : global::System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(global::System.String) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
            {
                var stringValue = value as string;
                if (stringValue is not null)
                {
                    return new VOTYPE(stringValue);
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(global::System.String) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(global::System.String))
                    {
                        return idValue.Value;
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }