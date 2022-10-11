using System;
using Vogen;

#pragma warning disable CS0219

// var x = Activator.CreateInstance<MyVo>();
// Console.WriteLine(x.Value);
Console.WriteLine("Hello world!");

var vo = CustomerId.From(123);
Console.WriteLine(vo.Value);

[ValueObject<int>]
public partial record CustomerId;

// [ValueObject(typeof(int))]
// public partial record MyVo2;


