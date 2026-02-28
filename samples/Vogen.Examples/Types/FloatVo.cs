using System;

namespace Vogen.Examples.Types
{
    [ValueObject<float>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public readonly partial struct Celsius { }

    [ValueObject<float>(conversions: Conversions.None)]
    public partial struct FloatVo { }

    [ValueObject<float>(conversions: Conversions.None)]
    public partial struct NoConverterFloatVo { }

    [ValueObject<float>(conversions: Conversions.TypeConverter)]
    public partial struct NoJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.NewtonsoftJson)]
    public partial struct NewtonsoftJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.SystemTextJson)]
    public partial struct SystemTextJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
    public partial struct BothJsonFloatVo { }

    [ValueObject<float>(conversions: Conversions.EfCoreValueConverter)]
    public partial struct EfCoreFloatVo { }

    [ValueObject<float>(conversions: Conversions.DapperTypeHandler)]
    public partial struct DapperFloatVo { }

    [ValueObject<float>(conversions: Conversions.LinqToDbValueConverter)]
    public partial struct LinqToDbFloatVo { }

    /// <summary>
    /// This example demonstrates that you can provide custom implementations for specific IConvertible methods.
    /// Vogen will not override your custom implementation; it will only hoist the other IConvertible methods.
    /// 
    /// In this case, we provide a custom ToInt32 implementation that rounds the float value,
    /// while other IConvertible methods (ToBoolean, ToByte, ToChar, etc.) are automatically hoisted.
    /// </summary>
    [ValueObject<float>(conversions: Conversions.None)]
    public partial struct CustomRoundingFloat
    {
        /// <summary>
        /// Custom implementation of IConvertible.ToInt32 that rounds instead of truncating.
        /// This demonstrates that Vogen respects user-defined implementations and won't regenerate them.
        /// </summary>
        public int ToInt32(IFormatProvider provider)
        {
            // Custom logic: round instead of truncate
            return IsInitialized() ? (int)Math.Round(Value) : 0;
        }
    }
}
