using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace ConsumerTests.BugFixTests.BugFix595;

[ValueObject]
public partial class Age_no_customization
{
}

[ValueObject(customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class Age_numbers_as_string_customization
{
}

public class Person_no_customization
{
    public Age_no_customization Age { get; set; } = Age_no_customization.From(0);
}

public class Person_numbers_as_string_customization
{
    public Age_numbers_as_string_customization Age { get; set; } = Age_numbers_as_string_customization.From(0);
}

/// <summary>
/// Fixes bug https://github.com/SteveDunn/Vogen/issues/595 where JsonNumberHandling
/// isn't treated as a flags enum in the generated code.
/// </summary>
public class Tests
{
    [Fact]
    public void Should_treat_number_as_string_when_specified_in_stj_options()
    {
        Person_no_customization person = new() { Age = Age_no_customization.From(42) };

        JsonSerializerOptions options = new()
        {
            NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
        };
        
        string s = SystemTextJsonSerializer.Serialize(person, options);
        s.Should().Be($$"""{"Age":"42"}""");
        var deserialized = SystemTextJsonSerializer.Deserialize<Person_no_customization>(s, options)!;

        deserialized.Age.Equals(person.Age).Should().BeTrue();
    }

    [Fact]
    public void Should_treat_number_as_string_when_specified_in_customization()
    {
        Person_numbers_as_string_customization person = new() { Age = Age_numbers_as_string_customization.From(42) };

        JsonSerializerOptions options = new()
        {
        };

        // it writes the number as a string
        SystemTextJsonSerializer.Serialize(person, options).Should().Be($$"""{"Age":"42"}""");
        
        
        // even though there are no STJ options to treat numbers as strings, we have provided the
        // [now obsolete] attribute to the value object (Person_string_customization), so the generator
        // writes the string version.
        var deserialized = SystemTextJsonSerializer.Deserialize<Person_numbers_as_string_customization>($$"""{"Age":"42"}""")!;

        deserialized.Age.Value.Should().Be(42);
    }
}
