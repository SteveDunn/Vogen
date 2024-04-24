#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(byte))]
[Instance("Manager", 1)]
[Instance("Operator", 2)]
public partial class EmployeeTypeByte
{
}

public class Byte
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeByte, List<Employee>> d = new()
        {
            { EmployeeTypeByte.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeByte.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeByte, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeByte.Manager);
        d2.Should().ContainKey(EmployeeTypeByte.Operator);

        d2[EmployeeTypeByte.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeByte.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif