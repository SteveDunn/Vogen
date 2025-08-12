using System;
using Vogen;

namespace NetStandard21Consumer
{
    [ValueObject]
    public partial struct Age
    {
    }

    [ValueObject(typeof(string))]
    public partial struct Name
    {
    }
    
    public class Class1
    {
    }
}