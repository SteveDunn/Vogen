using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Testbench.FooTests;
using Vogen;

namespace Testbench
{
    [System.ComponentModel.TypeConverter(typeof(NoJsonFooVoTypeConverter))]
    public struct NoJsonFooVo : System.IEquatable<NoJsonFooVo>
    {
#if DEBUG    
        private readonly System.Diagnostics.StackTrace _stackTrace = null!;
#endif

        private readonly bool _isInitialized;

        private readonly Bar _value;

        public readonly Bar Value
        {
            get
            {
                EnsureInitialized();
                return _value;
            }
        }

        public NoJsonFooVo()
        {
#if DEBUG
            _stackTrace = new System.Diagnostics.StackTrace();
#endif

            _isInitialized = false;
            _value = default!;
        }

        private NoJsonFooVo(Bar value)
        {
            _value = value;
            _isInitialized = true;
        }

        public static NoJsonFooVo From(Bar value)
        {
            NoJsonFooVo instance = new NoJsonFooVo(value);



            return instance;
        }

        public readonly bool Equals(NoJsonFooVo other)
        {
            // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
            // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
            if (!_isInitialized || !other._isInitialized) return false;

            return System.Collections.Generic.EqualityComparer<Bar>.Default.Equals(Value, other.Value);
        }

        public readonly bool Equals(Bar primitive) => Value.Equals(primitive);

        public readonly override bool Equals(object? obj)
        {
            return obj is NoJsonFooVo && Equals((NoJsonFooVo) obj);
        }

        public static bool operator ==(NoJsonFooVo left, NoJsonFooVo right) => Equals(left, right);
        public static bool operator !=(NoJsonFooVo left, NoJsonFooVo right) => !(left == right);

        public static bool operator ==(NoJsonFooVo left, Bar right) => Equals(left.Value, right);
        public static bool operator !=(NoJsonFooVo left, Bar right) => !Equals(left.Value, right);

        public static bool operator ==(Bar left, NoJsonFooVo right) => Equals(left, right.Value);
        public static bool operator !=(Bar left, NoJsonFooVo right) => !Equals(left, right.Value);

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<Bar>.Default.GetHashCode(_value);

        public readonly override string ToString() => Value.ToString()!;


        private readonly void EnsureInitialized()
        {
            if (!_isInitialized)
            {
#if DEBUG
                string message = "Use of uninitialized Value Object at: " + _stackTrace ?? "";
#else
                string message = "Use of uninitialized Value Object.";
#endif

                throw new ValueObjectValidationException(message);
            }
        }

        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<NoJsonFooVo, string>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null!)
                : base(
                    vo => System.Text.Json.JsonSerializer.Serialize(vo.Value, default(JsonSerializerOptions)),
                    text =>new NoJsonFooVo(System.Text.Json.JsonSerializer.Deserialize<Bar>(text, default(JsonSerializerOptions))),
                    mappingHints
                )
            { }
        }


        // public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<NoJsonFooVo>
        // {
        //     public override void SetValue(System.Data.IDbDataParameter parameter, NoJsonFooVo value)
        //     {
        //         var v = System.Text.Json.JsonSerializer.Serialize(value.Value);
        //         parameter.Value = v;
        //     }
        //
        //     public override NoJsonFooVo Parse(object value)
        //     {
        //         return value switch
        //         {
        //             string stringValue =>
        //                 new NoJsonFooVo(System.Text.Json.JsonSerializer.Deserialize<Bar>(stringValue)),
        //             _ => throw new System.InvalidCastException(
        //                 $"Unable to cast object of type {value.GetType()} to DapperFooVo")
        //         };
        //     }
        // }




        // class NoJsonFooVoTypeConverter : System.ComponentModel.TypeConverter
        // {
        //     public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        //     {
        //         return true;
        //     }
        //
        //     public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        //     {
        //         return new NoJsonFooVo((Bar)value);
        //         //return new NoJsonFooVo((Bar)base.ConvertFrom(()));
        //     }
        // }

        class NoJsonFooVoTypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, System.Type sourceType) => 
                sourceType == typeof(NoJsonFooVo);
                //sourceType == typeof(Bar) || sourceType == typeof(NoJsonFooVo) || base.CanConvertFrom(context, sourceType);

            public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value != null)
                {
                    if (value is Bar bar)
                    {
                        return new NoJsonFooVo(bar);
                    }

                    // if (value is NoJsonFooVo vo)
                    // {
                    //     return vo;
                    // }
                    //
                    // if (value is string s)
                    // {
                    //     return base.ConvertFrom(context, culture, s)!;
                    // }
                }

                return false;
                //return base.ConvertFrom(context, culture, value!)!;
            }

            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            {
                return destinationType == typeof(Bar) || destinationType == typeof(NoJsonFooVo);// || destinationType == typeof(string) ;//|| base.CanConvertTo(context, destinationType);
            }

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, System.Type destinationType)
            {
                if (value is NoJsonFooVo vo)
                {
                    if (destinationType == typeof(NoJsonFooVo)) return vo;
                    if (destinationType == typeof(Bar)) return vo.Value;
                    if (destinationType == typeof(string)) return base.ConvertTo(context, culture, vo, destinationType);
                }

                return null;

                //return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}