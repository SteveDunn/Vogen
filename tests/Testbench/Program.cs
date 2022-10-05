using System;
using Vogen;
#pragma warning disable CS0219

// var x = Activator.CreateInstance<MyVo>();
// Console.WriteLine(x.Value);
Console.WriteLine("Hello world!");


//[ValueObject(typeof(int))]
[ValueObject(typeof(int))]
public partial record MyVo
{
    // public override string ToString() => "hello!";
};

