#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(bool))]
[Instance("Manager", true)]
[Instance("Operator", false)]
public partial class EmployeeTypeBool
{
}

public class Bool
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeBool, List<Employee>> d = new()
        {
            { EmployeeTypeBool.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeBool.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeBool, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeBool.Manager);
        d2.Should().ContainKey(EmployeeTypeBool.Operator);

        d2[EmployeeTypeBool.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeBool.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif