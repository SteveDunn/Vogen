using Vogen;

// We don't need to emit the System.Text.Json converter factory because
// System.Text.Json, in the Infra project (or anything that references this)
// will have access to the 'fully formed' value objects.
// We also emit the static abstract interface, IVogen. Projects that reference this should use the 'Omit' option, otherwise they will get
// a compiler error because of duplicate definitions.
[assembly: VogenDefaults(
    systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit,
    staticAbstractsGeneration: StaticAbstractsGeneration.ValueObjectsDeriveFromTheInterface,
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