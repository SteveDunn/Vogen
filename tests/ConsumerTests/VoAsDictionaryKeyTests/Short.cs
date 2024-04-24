#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(short))]
[Instance("Manager", 1)]
[Instance("Operator", 2)]
public partial class EmployeeTypeShort
{
}

public class ShortTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeShort, List<Employee>> d = new()
        {
            { EmployeeTypeShort.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeShort.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeShort, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeShort.Manager);
        d2.Should().ContainKey(EmployeeTypeShort.Operator);

        d2[EmployeeTypeShort.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeShort.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif