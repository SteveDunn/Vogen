using System;
using Vogen;

namespace Whatever
{
    public class Program
    {
        public static void Main()
        {
            GeneratedValueObject c = Create(new Object[]
            {
                new()
            });
        }

        static GeneratedValueObject Create(Object[] normalObject)
        {
            return null!;//GeneratedValueObject.From(10);
        }
    }
    
    [ValueObject]
    partial class GeneratedValueObject {    }

    public abstract partial record class x(string X){}
}


