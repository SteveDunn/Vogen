using System;
using Vogen;

namespace NetFrameworkSample
{

    [ValueObject(conversions: Conversions.None)]
    public partial struct Age
    {
        
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Age is: {Age.From(42)}");
        }
    }
}