using System;
using System.Threading.Tasks;
using Vogen;

Console.WriteLine("!!");

[ValueObject()]
public partial class Whatever
{
    //public void Test(MyVo v = new MyVo()) {}
}


[ValueObject]
public partial struct MyVo
{
}

