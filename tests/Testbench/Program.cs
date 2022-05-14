using Vogen;

//[assembly: VogenDefaults(conversions: (Conversions)666)]

namespace Whatever;

public class Program
{
    public static void Main()
    {
    }
}

[ValueObject]
public partial record Age
{
}

[ValueObject]
[Instance("Min", 10, @"<abc>whatevs</abc>")]
internal sealed partial record MyVo
{
}
