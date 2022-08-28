using System.Threading.Tasks;
using Vogen;

namespace Testbench;

public class Program
{
    public static async Task Main()
    {
        await Task.CompletedTask;
    }
}

[ValueObject(typeof(float))]
[Instance("AbsoluteZero", -273.15f)]
public readonly partial struct Centigrade
{
}
