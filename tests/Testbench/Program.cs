using System;
using System.Text.RegularExpressions;
using Vogen;

namespace Whatever
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine(AccountId.From("xyz").ToString());
            // To debug the source generator or analyzer, set the active project to Vogen,
            // and then select Roslyn as the debug target. This requires the Roslyn SDK
            // to be installed in the list of Visual Studio components.
        }
    }

    [ValueObject(typeof(string))]
    public partial record AccountId
    {
        public sealed override string ToString()
        {
            return "!";
        }
    }

    public record Foo
    {
        public override string ToString() => "foo!";
    }
}