using Shared;
using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.PartialMethods;

public sealed class PartialMethodsTests
{
    [Fact]
    public Task Uses_method_partial_modifier_to_change_visibility()
    {
        const string source = """
            namespace Whatever;
            
            using Vogen;

            [ValueObject<string>]
            public partial class MyVo
            {
                private static partial MyVo From(string value);
                internal static partial bool TryFrom(string value, out MyVo result);
                private protected static partial ValueObjectOrError<MyVo> TryFrom(string value);
            }
            """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net9_0);
    }

    [Fact]
    public Task Uses_method_partial_modifier_to_add_attribute()
    {
        const string source = """
            namespace Whatever;

            using System;
            using Vogen;

            [ValueObject<string>]
            public partial struct MyVo
            {
                [Obsolete("Let's try an attribute")]
                public static partial MyVo From(string value);
            }
            """;

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .RunOn(TargetFramework.Net9_0);
    }
}