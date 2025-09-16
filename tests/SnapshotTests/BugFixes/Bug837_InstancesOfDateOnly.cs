using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/837
public class Bug837_InstancesOfDateOnly
{
    // Previously, any attribute found with an array as a parameter caused an
    // InvalidCastException in Vogen and stopped the generator.
    [Fact]
    public async Task Test_Date_Only()
    {
        var source = """

                     using System;
                     using Vogen;

                     [ValueObject<DateOnly>(conversions: Conversions.None)]
                     [Instance("Unspecified", "1900-01-01")]
                     public readonly partial struct TestDate;

                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOnAllFrameworks();
    }

    [Fact]
    public async Task Test_Time_Only()
    {
        var source = """

                     using System;
                     using Vogen;

                     [ValueObject<TimeOnly>(conversions: Conversions.None)]
                     [Instance("GinOClock", "16-30")]
                     public readonly partial struct TestTime;

                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .IgnoreInitialCompilationErrors()
                .RunOnAllFrameworks();
    }
}