using System;
using System.Threading.Tasks;
using Vogen;
#pragma warning disable CS0219

namespace Testbench;

public class Program
{
    public static async Task Main()
    {
        await Task.CompletedTask;


        char c = (char)'1';

        var x = MyVo.From(243);
        
        Console.WriteLine(x);
        Console.WriteLine(MyVo.i1);
        Console.WriteLine(MyVo.i2);
        Console.WriteLine(MyVo.i4);

        char c1 = '';
        char c2 = Convert.ToChar(1);

        bool b = c1 == c2;
    }
}

[ValueObject(typeof(byte))]
[Instance(name: "i1", value: 0)]
[Instance(name: "i2", value: 255)]
//[Instance(name: "i3", value: 256)]
[Instance(name: "i4", value: "255")]
//[Instance(name: "i5", value: "256")]
public partial struct MyVo
{
}