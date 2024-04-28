#nullable disable

using System.Text.Json;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(double))]
[Instance("Manager", 1)]
[Instance("Operator", 2)]
public partial class EmployeeTypeDouble
{
}

public class DoubleTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeDouble, List<Employee>> d = new()
        {
            { EmployeeTypeDouble.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeDouble.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeDouble, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeDouble.Manager);
        d2.Should().ContainKey(EmployeeTypeDouble.Operator);

        d2[EmployeeTypeDouble.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeDouble.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}
