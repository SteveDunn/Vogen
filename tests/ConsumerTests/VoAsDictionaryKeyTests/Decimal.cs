#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;
using Vogen;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(decimal))]
[Instance("Manager", 1)]
[Instance("Operator", 2)]
public partial class EmployeeTypeDecimal
{
}

public class DecimalTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeDecimal, List<Employee>> d = new()
        {
            { EmployeeTypeDecimal.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeDecimal.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeDecimal, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeDecimal.Manager);
        d2.Should().ContainKey(EmployeeTypeDecimal.Operator);

        d2[EmployeeTypeDecimal.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeDecimal.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif