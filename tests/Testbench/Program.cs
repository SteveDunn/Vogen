using System;
using Vogen;
#pragma warning disable CS0219

var x = Activator.CreateInstance<MyVo>();
 Console.WriteLine(x.Value);


[ValueObject(typeof(int))]
public partial record MyVo
{
    // public override string ToString() => "hello!";
};
