using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/854
public class Bug854_PropertiesNamedSystem
{
    [Fact]
    public async Task Without_a_namespace()
    {
        var source = """
                     using System;
                     using Vogen;

                     [ValueObject<Guid>]
                     public readonly partial struct UserId
                     {
                         public static readonly UserId Empty = new(Guid.Empty);
                         public static readonly UserId System = new(Guid.Empty);
                         public static UserId New() => From(Guid.NewGuid());
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }

    [Fact]
    public async Task With_a_namespace()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace MyNamespace;

                     [ValueObject<Guid>]
                     public readonly partial struct UserId
                     {
                         public static readonly UserId Empty = new(Guid.Empty);
                         public static readonly UserId System = new(Guid.Empty);
                         public static UserId New() => From(Guid.NewGuid());
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .IgnoreInitialCompilationErrors()
            .RunOnAllFrameworks();
    }
}