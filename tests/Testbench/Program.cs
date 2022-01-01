using System;
using System.Threading.Tasks;
using Vogen;

// Task<CustomerId> t = Task.FromResult<CustomerId>(new());
// Task<CustomerId> t2 = Task.FromResult<CustomerId>(default);

Foo<CustomerId> f = new Foo<CustomerId>();
var c = f.Get4();
Console.WriteLine(c.Value);

public class Foo<T> where T : struct
{
    public T Get4() => default;
}

[ValueObject(typeof(int))]
public partial struct CustomerId { }