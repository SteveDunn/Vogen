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

       bool b = MyIntVo.TryParse("123", out var r);
    }
}

[ValueObject(typeof(int))]
public partial struct MyIntVo
{
}