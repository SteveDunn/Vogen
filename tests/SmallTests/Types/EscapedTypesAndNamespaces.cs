#pragma warning disable IDE1006 // Naming Styles
using Vogen;

namespace record.@struct.@float
{
    public readonly record struct @decimal();
}

namespace @double
{
    public readonly record struct @decimal();

    [ValueObject(typeof(@decimal))]
    public partial class classFromEscapedNamespaceWithReservedUnderlyingType
    {
    }

    [ValueObject]
    public partial class classFromEscapedNamespace
    {
    }
}

namespace @bool.@byte.@short.@float.@object
{
    [ValueObject]
    public partial class @class
    {
    }

    [ValueObject]
    public partial class @event
    {
    }

    [ValueObject(typeof(record.@struct.@float.@decimal))]
    public partial class @event2
    {
    }
}