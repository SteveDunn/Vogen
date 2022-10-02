using System;
using System.Threading.Tasks;
using Vogen;

namespace Testbench;

public class Program
{
    public static async Task Main()
    {
        var x = LinqToDbStringVo.From("123");
        var y = new LinqToDbStringVo.LinqToDbValueConverter();
        

            // var x = new MyIntVo();
        await Task.CompletedTask;

        // var vo = Activator.CreateInstance<MyIntVo>();
        //var vo = (MyIntVo)Activator.CreateInstance(typeof(MyIntVo))!;
        //Console.WriteLine(vo.Value);
    }
}

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(string))]
public partial class LinqToDbStringVo { }


[ValueObject(conversions: Conversions.None, underlyingType: typeof(int))]
public readonly partial struct MyIntVo { }

[ValueObject]
public partial class Age
{
}