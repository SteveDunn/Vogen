// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantVerbatimPrefix
#pragma warning disable IDE1006 // Naming Styles
namespace record.@struct.@float
{
    public readonly record struct @decimal;
}

namespace @double
{
    public readonly record struct @decimal;

    [ValueObject(typeof(@decimal))]
    public partial class classFromEscapedNamespaceWithReservedUnderlyingType
    {
    }

    [ValueObject<int>]
    public partial class classFromEscapedNamespace
    {
    }

    [EfCoreConverter<classFromEscapedNamespace>]
    [EfCoreConverter<classFromEscapedNamespaceWithReservedUnderlyingType>]
    public partial class EfCoreConverters;
}

namespace @bool.@byte.@short.@float.@object
{
    [ValueObject<int>]
    public partial class @class
    {
    }

    [ValueObject<int>]
    public partial class @event
    {
    }

    [ValueObject(typeof(record.@struct.@float.@decimal))]
    public partial class @event2
    {
    }
    
    [EfCoreConverter<@class>]
    [EfCoreConverter<@event>]
    [EfCoreConverter<@event2>]
    public partial class EfCoreConverters;
}