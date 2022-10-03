using System;
using Vogen;
#pragma warning disable CS0219

MyVo? c = default;
MyVo? c2 = default(MyVo);
Console.WriteLine(c!.Value);
Console.WriteLine(c2!.Value);

[ValueObject(typeof(int))]
public partial class MyVo { }

public class Test
{
    public Test()
    {
        MyVo c = default!;
        MyVo c2 = default(MyVo)!;
    }

    public MyVo Get() => default!;
}
