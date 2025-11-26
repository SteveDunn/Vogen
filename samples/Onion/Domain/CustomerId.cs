using Vogen;

namespace Domain;

[ValueObject]
public partial struct CustomerId
{
    public static bool operator <(CustomerId left, CustomerId right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(CustomerId left, CustomerId right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(CustomerId left, CustomerId right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(CustomerId left, CustomerId right)
    {
        return left.CompareTo(right) >= 0;
    }
}