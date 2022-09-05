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
[Instance(name: "i2", value: "2.34x")]
public readonly partial struct MyInstance
{
}