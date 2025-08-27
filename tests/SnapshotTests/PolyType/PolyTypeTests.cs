using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.PolyType;

public class PolyTypeTests
{
    [Fact]
    public async Task PolyType_support_is_not_generated_without_PolyType_reference()
    {
        var source =
            """
            using System;
            using Vogen;
            
            namespace Whatever;
            
            [ValueObject<int>]
            public partial struct MyId;
            """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }

    [Fact]
    public async Task PolyType_support_is_generated_with_PolyType_reference()
    {
        var source =
            """
            using System;
            using Vogen;

            // Simulate having PolyType available by defining the attribute
            namespace PolyType
            {
                public class TypeShapeAttribute : System.Attribute
                {
                    public System.Type? Marshaler { get; set; }
                    public TypeShapeKind Kind { get; set; }
                }
                
                public enum TypeShapeKind
                {
                    None
                }
                
                public interface IMarshaler<T, TUnderlying>
                {
                    TUnderlying Marshal(T value);
                    T Unmarshal(TUnderlying value);
                }
            }
            
            [ValueObject<int>]
            public partial struct MyId;
            """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net8_0);
    }
}
