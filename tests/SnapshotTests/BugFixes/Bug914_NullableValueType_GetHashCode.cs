using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.BugFixes;

// See https://github.com/SteveDunn/Vogen/issues/914
// ValueObject with a nullable value type underlying (e.g. ushort?) generated CS8607 because
// EqualityComparer<T>.Default.GetHashCode has [DisallowNull] in .NET 10+.
public class Bug914Tests
{
    [Theory]
    [InlineData("partial struct")]
    [InlineData("readonly partial struct")]
    [InlineData("partial class")]
    [InlineData("partial record class")]
    [InlineData("partial record struct")]
    [InlineData("readonly partial record struct")]
    public async Task NullableValueType_underlying_generates_null_safe_GetHashCode(string type)
    {
        var source = $$"""
                     using Vogen;
                     
                     namespace ConsumerTests;
                     
                     [ValueObject<ushort?>]
                     internal {{type}} Speed
                     {
                         private const ushort UnknownSpeed = 255;
                     
                         private static ushort? NormalizeInput(ushort? input) =>
                             input == UnknownSpeed ? null : input;
                     }
                     """;

        await new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(s => s.UseHashedParameters(type))
            .RunOnAllFrameworks();
    }
}
