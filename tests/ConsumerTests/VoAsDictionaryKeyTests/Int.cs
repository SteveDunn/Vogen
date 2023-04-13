#if NET6_0_OR_GREATER

#nullable disable

using System.Text.Json;
using Vogen;

namespace ConsumerTests.VoAsDictionaryKeyTests;


[ValueObject]
[Instance("Manager", 1)]
[Instance("Operator", 2)]
public partial class EmployeeTypeInt
{
}

public class IntTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeInt, List<Employee>> d = new()
        {
            { EmployeeTypeInt.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeInt.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeInt, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeInt.Manager);
        d2.Should().ContainKey(EmployeeTypeInt.Operator);

        d2[EmployeeTypeInt.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeInt.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif