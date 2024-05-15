namespace Vogen.Examples.SerializationAndConversion.EFCore;

public class PersonEntity
{
    public Id Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public Name Name { get; set; } = Name.NotSet;
    public Age Age { get; set; }
}

/// <summary>
/// Converter needed because it's a class
/// </summary>
[ValueObject(conversions: Conversions.EfCoreValueConverter)]
public partial class Id
{
}

[ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
[Instance("NotSet", "[NOT_SET]")]
public partial class Name
{
}

/// <summary>
/// No converter needed because it's a struct of a supported type
/// </summary>
[ValueObject]
public partial struct Age
{
}