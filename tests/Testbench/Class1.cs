﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a source generator named Vogen (https://github.com/SteveDunn/Vogen)
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Vogen;

namespace Vogen.IntegrationTests.TestTypes
{
    [System.Diagnostics.DebuggerTypeProxyAttribute(typeof(NoConverterByteVoDebuggerProxy))]
    [System.Diagnostics.DebuggerDisplayAttribute("Underlying type: byte, Value={_value}")]
    public partial struct NoConverterByteVo : System.IEquatable<NoConverterByteVo>
    {

        public class NoConverterByteVoDebuggerProxy
        {
            private readonly NoConverterByteVo _t;

            NoConverterByteVoDebuggerProxy(NoConverterByteVo t)
            {
                _t = t;
            }

            public bool IsInitialized => _t._isInitialized;
            public string Value => _t._isInitialized ? _t._value.ToString() : "[not initialized]" ;
            
            #if DEBUG
            public string CreatedWith =>  _t._stackTrace?.ToString() ?? "the From method";
            #endif

            public string Conversions => "{Util.GenerateAnyConversionAttributes(tds, item)}";
        }
#if DEBUG    
        private readonly System.Diagnostics.StackTrace _stackTrace = null;
#endif

        private readonly bool _isInitialized;

        private readonly byte _value;

        public readonly byte Value
        {
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {
                EnsureInitialized();
                return _value;
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        [System.Diagnostics.DebuggerHidden]
        public NoConverterByteVo()
        {
#if DEBUG
            _stackTrace = new System.Diagnostics.StackTrace();
#endif

            _isInitialized = false;
            _value = default;
        }

        [System.Diagnostics.DebuggerStepThroughAttribute]
        [System.Diagnostics.DebuggerHidden]
        private NoConverterByteVo(byte value)
        {
            _value = value;
            _isInitialized = true;
        }

        public static NoConverterByteVo From(byte value)
        {
            NoConverterByteVo instance = new NoConverterByteVo(value);



            return instance;
        }

        public readonly bool Equals(NoConverterByteVo other)
        {
            // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
            // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
            if (!_isInitialized || !other._isInitialized) return false;

            return System.Collections.Generic.EqualityComparer<byte>.Default.Equals(Value, other.Value);
        }

        public readonly bool Equals(byte primitive) => Value.Equals(primitive);

        public readonly override bool Equals(object obj)
        {
            return obj is NoConverterByteVo && Equals((NoConverterByteVo) obj);
        }

        public static bool operator ==(NoConverterByteVo left, NoConverterByteVo right) => Equals(left, right);
        public static bool operator !=(NoConverterByteVo left, NoConverterByteVo right) => !(left == right);

        public static bool operator ==(NoConverterByteVo left, byte right) => Equals(left.Value, right);
        public static bool operator !=(NoConverterByteVo left, byte right) => !Equals(left.Value, right);

        public static bool operator ==(byte left, NoConverterByteVo right) => Equals(left, right.Value);
        public static bool operator !=(byte left, NoConverterByteVo right) => !Equals(left, right.Value);

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<byte>.Default.GetHashCode(_value);

        public readonly override string ToString() => Value.ToString();

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










    }
}