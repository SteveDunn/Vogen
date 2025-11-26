using Vogen;

namespace Domain;

[ValueObject]
public readonly partial struct Age
{
    public static bool operator <(Age left, Age right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Age left, Age right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Age left, Age right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Age left, Age right)
    {
        return left.CompareTo(right) >= 0;
    }
}