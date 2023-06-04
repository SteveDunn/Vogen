#if NET6_0_OR_GREATER
#nullable disable

using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vogen;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(DateTime))]
[Instance("Manager", 638064864000000000L)]
[Instance("Operator", 2147483647)]
public partial class EmployeeTypeDateTime
{
}

public class DateTimeTests
{
    [Fact]
    public void can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeDateTime, List<Employee>> startingDictionary = new()
        {
            { EmployeeTypeDateTime.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeDateTime.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(startingDictionary);

        var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<EmployeeTypeDateTime, List<Employee>>>(json);

        deserializedDictionary.Should().ContainKey(EmployeeTypeDateTime.Manager);
        deserializedDictionary.Should().ContainKey(EmployeeTypeDateTime.Operator);

        deserializedDictionary[EmployeeTypeDateTime.Manager].Should().Contain(new Employee("John Smith", 30));
        deserializedDictionary[EmployeeTypeDateTime.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif