using System;
using System.Threading.Tasks;
using Vogen;

namespace Testbench;

public class Program
{
    public static void Main()
    {
        await Task.CompletedTask;

        // var vo = Activator.CreateInstance<MyIntVo>();
        //var vo = (MyIntVo)Activator.CreateInstance(typeof(MyIntVo))!;
        //Console.WriteLine(vo.Value);
    }
}

[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public readonly partial struct MyIntVo { }