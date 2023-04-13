#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;
using Vogen;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(TimeOnly))]
public partial class EmployeeTypeTimeOnly
{
    public static readonly EmployeeTypeTimeOnly Manager =  EmployeeTypeTimeOnly.From(new TimeOnly(12, 13, 59));
    public static readonly EmployeeTypeTimeOnly Operator = EmployeeTypeTimeOnly.From(new TimeOnly(14, 30, 59));
}

public class TimeOnlyTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeTimeOnly, List<Employee>> d = new()
        {
            { EmployeeTypeTimeOnly.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeTimeOnly.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeTimeOnly, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeTimeOnly.Manager);
        d2.Should().ContainKey(EmployeeTypeTimeOnly.Operator);

        d2[EmployeeTypeTimeOnly.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeTimeOnly.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif