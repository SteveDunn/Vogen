#if NET6_0_OR_GREATER
#nullable disable

using System.Text.Json;
using Vogen;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(Guid))]
public partial class EmployeeTypeGuid
{
    public static readonly EmployeeTypeGuid Manager = EmployeeTypeGuid.From(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static readonly EmployeeTypeGuid Operator = EmployeeTypeGuid.From(Guid.Parse("00000000-0000-0000-0000-000000000002"));
}

public class GuidTests
{
    [Fact]
    public void int_can_serialize_value_object_as_key_of_dictionary()
    {
        Dictionary<EmployeeTypeGuid, List<Employee>> d = new()
        {
            { EmployeeTypeGuid.Manager, new List<Employee> { new Employee("John Smith", 30) } },
            { EmployeeTypeGuid.Operator, new List<Employee> { new Employee("Dave Angel", 42) } }
        };

        var json = JsonSerializer.Serialize(d);

        var d2 = JsonSerializer.Deserialize<Dictionary<EmployeeTypeGuid, List<Employee>>>(json);

        d2.Should().ContainKey(EmployeeTypeGuid.Manager);
        d2.Should().ContainKey(EmployeeTypeGuid.Operator);

        d2[EmployeeTypeGuid.Manager].Should().Contain(new Employee("John Smith", 30));
        d2[EmployeeTypeGuid.Operator].Should().Contain(new Employee("Dave Angel", 42));
    }
}

#endif