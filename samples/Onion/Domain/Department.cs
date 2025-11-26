using Vogen;

namespace Domain;

[ValueObject<string>]
public readonly partial record struct Department
{
    public static bool operator <(Department left, Department right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Department left, Department right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Department left, Department right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Department left, Department right)
    {
        return left.CompareTo(right) >= 0;
    }
}