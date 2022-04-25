﻿
        class VOTYPETypeConverter : global::System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(global::System.Guid) || sourceType == typeof(global::System.String) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    System.Guid guidValue => new VOTYPE(guidValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new VOTYPE(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(global::System.Guid) || sourceType == typeof(global::System.String) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(global::System.Guid))
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