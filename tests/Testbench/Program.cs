using System;
using Vogen;
#pragma warning disable CS0219

// var x = Activator.CreateInstance<MyVo>();
// Console.WriteLine(x.Value);
Console.WriteLine("Hello world!");

var gt = new GenericThing<MyVo>();
var vo = gt.GetNewItem();
    Console.WriteLine(vo.Value);
//gt.Set(default);

//[ValueObject(typeof(int))]
[ValueObject(typeof(int))]
public partial record MyVo
{
    // public override string ToString() => "hello!";
};

public class GenericThing<T> where T : new()
{
    public T GetNewItem()
    {
        return new T();
    }
}

