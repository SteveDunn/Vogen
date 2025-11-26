using Vogen;

namespace Domain;

[ValueObject<string>]
public partial struct CustomerName
{
    public static bool operator <(CustomerName left, CustomerName right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(CustomerName left, CustomerName right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(CustomerName left, CustomerName right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(CustomerName left, CustomerName right)
    {
        return left.CompareTo(right) >= 0;
    }
}