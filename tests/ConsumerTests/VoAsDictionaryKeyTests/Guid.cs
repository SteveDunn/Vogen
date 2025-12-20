#nullable disable

using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions.Extensions;

namespace ConsumerTests.VoAsDictionaryKeyTests;

[ValueObject(typeof(Guid), customizations: Customizations.AddFactoryMethodForGuids)]
public partial class EmployeeTypeGuid
{
    public static readonly EmployeeTypeGuid Manager = EmployeeTypeGuid.From(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static readonly EmployeeTypeGuid Operator = EmployeeTypeGuid.From(Guid.Parse("00000000-0000-0000-0000-000000000002"));
}

public class GuidTests
{
    [Fact]
    public async Task Factory_for_V7_is_supported()
    {
        var g1 = EmployeeTypeGuid.FromNewVersion7Guid();
        await Task.Delay(100);
        var g2 = EmployeeTypeGuid.FromNewVersion7Guid();
        await Task.Delay(100);
        var g3 = EmployeeTypeGuid.FromNewVersion7Guid();
        
        g1.Should().BeLessThan(g2);
        g2.Should().BeLessThan(g3);
    }

    [Fact]
    public void Factory_for_V7_with_explicit_time_is_supported()
    {
        var latest = new DateTimeOffset(2025, 12, 20, 10, 05, 06, TimeSpan.Zero);
        var g1 = EmployeeTypeGuid.FromNewVersion7Guid(latest);
        var g2 = EmployeeTypeGuid.FromNewVersion7Guid(latest - 1.Seconds());
        var g3 = EmployeeTypeGuid.FromNewVersion7Guid(latest - 2.Seconds());
        
        g2.Should().BeLessThan(g1);
        g3.Should().BeLessThan(g2);
    }

    [Fact]
    public void Factory_for_standard_guid_is_supported()
    {
        var g1 = EmployeeTypeGuid.FromNewGuid();
        var g2 = EmployeeTypeGuid.FromNewGuid();
        var g3 = EmployeeTypeGuid.FromNewGuid();
        
        g1.ToString().Should().NotBe(g2.ToString());
        g2.ToString().Should().NotBe(g3.ToString());
    }

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
