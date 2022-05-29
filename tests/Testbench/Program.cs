using @class.@struct.@int;
using Vogen;

namespace Whatever
{
    public class Program
    {
        public static void Main()
        {
            var r = @float.From(11);
        }
    }
    
    [ValueObject(typeof(int))]
    public partial class Age
    {
    }

}

namespace record.@int
{
    public class @decimal
    {
    }
}

namespace @class.@struct.@int
{
    [ValueObject(typeof(decimal))]
    [Instance("onetwothree", 123)]
    public partial class @float
    {
    }
}