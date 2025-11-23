using System;
using Vogen;

namespace Types
{
    [ValueObject]
    public partial struct Age
    {
    }

    [ValueObject(typeof(string))]
    public partial struct Name
    {
    }
    
    public class Types
    {
    }
}