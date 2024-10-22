using Vogen;
using Vogen.EfCoreTest;

[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
    conversions: Conversions.EfCoreValueConverter)]

namespace Testbench;

public static class Program
{
    public static void Main()
    {
        // EfCoreScenario.Run();
        ToStringScenario.Runner.Run();
    }
}

[ValueObject<string>]
internal sealed partial class T3 {
    public bool Equals(T3? other) => Value == other?.Value;

    public override int GetHashCode() => 0;
}