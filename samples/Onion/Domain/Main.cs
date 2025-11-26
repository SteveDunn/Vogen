using Vogen;

// We don't need to emit the System.Text.Json converter factory because
// System.Text.Json, in the Infra project (or anything that references this)
// will have access to the 'fully formed' value objects.
[assembly: VogenDefaults(
    systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
    conversions: Conversions.SystemTextJson | 
                 Conversions.TypeConverter)]

namespace Domain;

[ValueObject<DateOnly>]
public partial record class HireDate
{
    public static bool operator <(HireDate left, HireDate right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(HireDate left, HireDate right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(HireDate left, HireDate right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(HireDate left, HireDate right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}