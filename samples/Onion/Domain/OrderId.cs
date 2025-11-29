using Vogen;

namespace Domain;

[ValueObject]
public partial struct OrderId
{
    public static bool operator <(OrderId left, OrderId right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(OrderId left, OrderId right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(OrderId left, OrderId right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(OrderId left, OrderId right)
    {
        return left.CompareTo(right) >= 0;
    }
}