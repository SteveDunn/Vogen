#nullable disable
using FluentAssertions;
using Vogen;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MediumTests.SerializationAndConversionTests;

[ValueObject(underlyingType: typeof(double), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class DoubleHolderId_string
{
}

[ValueObject(underlyingType: typeof(decimal), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class DecimalHolderId_string
{
}

[ValueObject(underlyingType: typeof(float), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class FloatHolderId_string
{
}

[ValueObject(underlyingType: typeof(long), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class LongHolderId_string
{
}

[ValueObject(underlyingType: typeof(short), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class ShortHolderId_string
{
}

[ValueObject(underlyingType: typeof(int), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class IntHolderId_string
{
}

[ValueObject(underlyingType: typeof(byte), customizations: Customizations.TreatNumberAsStringInSystemTextJson)]
public partial class ByteHolderId_string
{
}

[ValueObject(underlyingType: typeof(double))]
public partial class DoubleHolderId_normal
{
}

[ValueObject(underlyingType: typeof(decimal))]
public partial class DecimalHolderId_normal
{
}

[ValueObject(underlyingType: typeof(float))]
public partial class FloatHolderId_normal
{
}

[ValueObject(underlyingType: typeof(long))]
public partial class LongHolderId_normal
{
}

[ValueObject(underlyingType: typeof(short))]
public partial class ShortHolderId_normal
{
}

[ValueObject(underlyingType: typeof(int))]
public partial class IntHolderId_normal
{
}

[ValueObject(underlyingType: typeof(byte))]
public partial class ByteHolderId_normal
{
}
    
public class Container
{
    public DoubleHolderId_string DoubleHolder_as_a_string { get; set; } = null!;
    public DecimalHolderId_string DecimalHolder_as_a_string { get; set; } = null!;
    public LongHolderId_string LongHolder_as_a_string { get; set; } = null!;
    public FloatHolderId_string FloatHolder_as_a_string { get; set; } = null!;
    public ByteHolderId_string ByteHolder_as_a_string { get; set; } = null!;
    public IntHolderId_string IntHolder_as_a_string { get; set; } = null!;
    public ShortHolderId_string ShortHolder_as_a_string { get; set; } = null!;

    public DoubleHolderId_normal DoubleHolder_normal { get; set; } = null!;
    public DecimalHolderId_normal DecimalHolder_normal { get; set; } = null!;
    public LongHolderId_normal LongHolder_normal { get; set; } = null!;
    public FloatHolderId_normal FloatHolder_normal { get; set; } = null!;
    public ByteHolderId_normal ByteHolder_normal { get; set; } = null!;
    public IntHolderId_normal IntHolder_normal { get; set; } = null!;
    public ShortHolderId_normal ShortHolder_normal { get; set; } = null!;
}

public class CustomizationTests
{
    [Fact]
    public void CanSerializeAndDeserializeAsString()
    {
        var holderId = DoubleHolderId_string.From(42);

        string serialized = JsonSerializer.Serialize(holderId);
        var deserialized = JsonSerializer.Deserialize<DoubleHolderId_string>(serialized);

        deserialized.Value.Should().Be(42);
    }

    [Fact]
    public void CanSerializeAndDeserializeWhenVoIsInAComplexObject()
    {
        var container  = new Container
        {
            ByteHolder_as_a_string = ByteHolderId_string.From(123),
            DecimalHolder_as_a_string = DecimalHolderId_string.From(720742592373919744),
            DoubleHolder_as_a_string = DoubleHolderId_string.From(720742592373919744),
            FloatHolder_as_a_string = FloatHolderId_string.From(720742592373919744),
            IntHolder_as_a_string = IntHolderId_string.From(321),
            LongHolder_as_a_string = LongHolderId_string.From(720742592373919744),
            ShortHolder_as_a_string = ShortHolderId_string.From(123),

            ByteHolder_normal = ByteHolderId_normal.From(123),
            DecimalHolder_normal = DecimalHolderId_normal.From(720742592373919744),
            DoubleHolder_normal = DoubleHolderId_normal.From(720742592373919744),
            FloatHolder_normal = FloatHolderId_normal.From(720742592373919744),
            IntHolder_normal = IntHolderId_normal.From(321),
            LongHolder_normal = LongHolderId_normal.From(720742592373919744),
            ShortHolder_normal = ShortHolderId_normal.From(123),
        };
        
        string serialized = JsonSerializer.Serialize(container);

        serialized.Should().Be("{\"DoubleHolder_as_a_string\":\"7.2074259237392E\\u002B17\",\"DecimalHolder_as_a_string\":\"720742592373919744\",\"LongHolder_as_a_string\":\"720742592373919744\",\"FloatHolder_as_a_string\":\"7.207426E\\u002B17\",\"ByteHolder_as_a_string\":\"123\",\"IntHolder_as_a_string\":\"321\",\"ShortHolder_as_a_string\":\"123\",\"DoubleHolder_normal\":7.2074259237391974E+17,\"DecimalHolder_normal\":720742592373919744,\"LongHolder_normal\":720742592373919744,\"FloatHolder_normal\":7.20742585E+17,\"ByteHolder_normal\":123,\"IntHolder_normal\":321,\"ShortHolder_normal\":123}");
        
        var deserialized = JsonSerializer.Deserialize<Container>(serialized);

        deserialized.ByteHolder_as_a_string.Value.Should().Be(123);
        deserialized.ByteHolder_normal.Value.Should().Be(123);
    }
}
