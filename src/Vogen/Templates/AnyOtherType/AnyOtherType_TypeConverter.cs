
        class VOTYPETypeConverter : global::System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(VOUNDERLYINGTYPE);
            }
        
            public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value)
            {
                VOUNDERLYINGTYPE ut = (VOUNDERLYINGTYPE)value;

                return new VOTYPE(ut);
            }
        
            public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Type sourceType)
            {
                return sourceType == typeof(VOUNDERLYINGTYPE);
            }
        
            public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, global::System.Object value, global::System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    return idValue.Value;
                }
        
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
