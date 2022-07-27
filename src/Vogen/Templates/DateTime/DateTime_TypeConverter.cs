
        class VOTYPETypeConverter : global::System.ComponentModel.TypeConverter
        {
            public override global::System.Boolean CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(global::System.DateTime) || sourceType == typeof(global::System.String) || base.CanConvertFrom(context, sourceType);
            }

            public override global::System.Object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value)
            {
                return value switch
                {
                    global::System.String stringValue when !global::System.String.IsNullOrEmpty(stringValue) && global::System.DateTime.TryParseExact(stringValue, "O", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.RoundtripKind, out var result) => VOTYPE.Deserialize(result),
                    global::System.DateTime dateTimeValue => VOTYPE.Deserialize(dateTimeValue),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(global::System.DateTime) || sourceType == typeof(global::System.String) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value, global::System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(global::System.DateTime))
                    {
                        return idValue.Value;
                    }

                    if (destinationType == typeof(global::System.String))
                    {
                        return idValue.Value.ToString("O");
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }