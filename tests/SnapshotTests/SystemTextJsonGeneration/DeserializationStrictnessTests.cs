using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.SystemTextJsonGeneration;

public class DeserializationStrictnessTests
{
    [Fact]
    public Task Reference_value_objects_allow_nulls_by_default()
    {
        var source = """
                     using Vogen;
                     namespace Whatever;
                     
                     [ValueObject]
                     public partial class ReferenceVo;
                     """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }

    [Fact]
    public Task Can_disallow_nulls_for_reference_value_objects()
    {
        var source = """
                     using Vogen;
                     namespace Whatever;
                     
                     [ValueObject(deserializationStrictness: DeserializationStrictness.DisallowNulls)]
                     public partial class ReferenceVo;
                     """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOnAllFrameworks();
    }
    
}