using System.Text.Json;
using System.Text.Json.Serialization;
using AotTrimmedSample;
using Vogen;

// to *not* generate the factory, use:
// [assembly: VogenDefaults(systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit)]

Person person = new()
{
    Name = Name.From("Fred Flintstone"),
    Age = Age.From(44),
    Address = Address.From("201 Cobblestone Lane"),
};

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    Converters =
    {
        new VogenTypesFactory()
    }
};

var ctx = new JsonSourceGenerationContext(options);

var json = JsonSerializer.Serialize(person, ctx.Person);
Person person2 = JsonSerializer.Deserialize(json, ctx.Person)!;

Console.WriteLine(json);
Console.WriteLine($"{person2.Name} is {person2.Age}, and lives at {person2.Address}");

public class Person
{
    public Age Age { get; set; }
    public Name Name { get; set; }
    public Address Address { get; set; }
}

[ValueObject<int>]
public partial struct Age {}

[ValueObject<string>]
public partial struct Name {}

[ValueObject<string>]
public partial struct Address {}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Person))]
internal partial class JsonSourceGenerationContext : JsonSerializerContext
{
}
